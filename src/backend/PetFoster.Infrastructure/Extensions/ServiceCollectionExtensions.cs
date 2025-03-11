using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minio;
using PetFoster.Application.Database;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Messaging;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using PetFoster.Infrastructure.BackgroundServices;
using PetFoster.Infrastructure.Files;
using PetFoster.Infrastructure.MessageQueues;
using PetFoster.Infrastructure.Options;
using PetFoster.Infrastructure.Providers;
using PetFoster.Infrastructure.Repositories;
using System.Text.Json;

namespace PetFoster.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string DATABASE_NAME = "Database";

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, 
            IConfiguration configuration)
        {            
            services.AddDbContext<ApplicationDbContext>(opt =>
            {                
                opt.UseNpgsql(configuration.GetConnectionString(DATABASE_NAME));
                opt.UseSnakeCaseNamingConvention();
                opt.EnableSensitiveDataLogging();
                opt.UseLoggerFactory(CreateLoggerFactory());                
                opt.ConfigureWarnings(warnings => warnings.Ignore(
                    RelationalEventId.PendingModelChangesWarning));
            });

            services.AddMinioServices(configuration);

            services.AddHostedService<FilesCleanerBackgroundService>();

            services.AddSingleton<IMessageQueue<IEnumerable<Application.Files.FileInfo>>, InMemoryMessageQueue<IEnumerable<Application.Files.FileInfo>>>();
            
            services.AddScoped<IRepository<Volunteer, VolunteerId>, VolunteersRepository>();
            services.AddScoped<IRepository<Specie, SpecieId>, SpeciesRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IFilesCleanerService, FilesCleanerService>();

            return services;
        }

        private static IServiceCollection AddMinioServices(this IServiceCollection services, 
            IConfiguration configuration)
        {   
            services.AddMinio(options =>
            {
                var minioOptions = configuration.GetSection(MinioOptions.MINIO).Get<MinioOptions>()
                ?? throw new ApplicationException("Missing minio configuration");

                options.WithEndpoint(minioOptions.Endpoint);
                options.WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey);
                options.WithSSL(minioOptions.WithSsl);
            });

            services.AddScoped<IFileProvider, MinioProvider>();

            return services;
        }

        private static ILoggerFactory CreateLoggerFactory()
            => LoggerFactory.Create(builder => builder.AddConsole());

        public static PropertyBuilder<IReadOnlyList<T>> JsonValueObjectCollectionConversion<T>(
            this PropertyBuilder<IReadOnlyList<T>> builder)
        {
            return builder.HasConversion<string>(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<IReadOnlyList<T>>(v, JsonSerializerOptions.Default)!,
                new ValueComparer<IReadOnlyList<T>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a,v) => HashCode.Combine(a, v!.GetHashCode())),
                    c => c.ToList()));
        }
    }
}
