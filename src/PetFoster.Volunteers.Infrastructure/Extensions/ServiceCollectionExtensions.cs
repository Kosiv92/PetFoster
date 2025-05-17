using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minio;
using PetFoster.Application.Interfaces;
using PetFoster.Core.Database;
using PetFoster.Core.Interfaces;
using PetFoster.Volunteers.Application;
using PetFoster.Volunteers.Infrastructure;
using PetFoster.Volunteers.Infrastructure.BackgroundServices;
using PetFoster.Volunteers.Infrastructure.DbContext;
using System.Text.Json;

namespace PetFoster.Volunteers.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            _ = services.AddDbContext<WriteDbContext>(opt =>
            {
                _ = opt.UseNpgsql(configuration.GetConnectionString(Constants.DATABASE_NAME));
                _ = opt.UseSnakeCaseNamingConvention();
                _ = opt.EnableSensitiveDataLogging();
                _ = opt.UseLoggerFactory(CreateLoggerFactory());
                _ = opt.ConfigureWarnings(warnings => warnings.Ignore(
                    RelationalEventId.PendingModelChangesWarning));
            });

            _ = services.AddDbContext<ReadDbContext>(opt =>
            {
                _ = opt.UseNpgsql(configuration.GetConnectionString(Constants.DATABASE_NAME));
                _ = opt.UseSnakeCaseNamingConvention();
                _ = opt.EnableSensitiveDataLogging();
                _ = opt.UseLoggerFactory(CreateLoggerFactory());
                _ = opt.ConfigureWarnings(warnings => warnings.Ignore(
                    RelationalEventId.PendingModelChangesWarning));
            });

            _ = services.AddMinioServices(configuration);

            _ = services.AddHostedService<FilesCleanerBackgroundService>();

            _ = services.AddSingleton<IMessageQueue<IEnumerable<Application.Files.FileInfo>>, InMemoryMessageQueue<IEnumerable<Application.Files.FileInfo>>>();
            _ = services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

            _ = services.AddScoped<IRepository<Volunteer, VolunteerId>, VolunteersRepository>();
            _ = services.AddScoped<IRepository<Specie, SpecieId>, SpeciesWriteRepository>();
            _ = services.AddScoped<IVolunteersQueryRepository, VolunteersQueryRepository>();
            _ = services.AddKeyedScoped<ISpeciesQueryRepository, SpeciesQueryRepository>("dapper");
            _ = services.AddKeyedScoped<ISpeciesQueryRepository, SpeciesReadRepository>("ef");
            _ = services.AddScoped<IUnitOfWork, UnitOfWork>();

            _ = services.AddTransient<IFilesCleanerService, FilesCleanerService>();

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            return services;
        }

        private static IServiceCollection AddMinioServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            _ = services.AddMinio(options =>
            {
                MinioOptions minioOptions = configuration.GetSection(MinioOptions.MINIO).Get<MinioOptions>()
                ?? throw new ApplicationException("Missing minio configuration");

                _ = options.WithEndpoint(minioOptions.Endpoint);
                _ = options.WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey);
                _ = options.WithSSL(minioOptions.WithSsl);
            });

            _ = services.AddScoped<IFileProvider, MinioProvider>();

            return services;
        }

        private static ILoggerFactory CreateLoggerFactory()
        {
            return LoggerFactory.Create(builder => builder.AddConsole());
        }

        public static PropertyBuilder<IReadOnlyList<T>> JsonValueObjectCollectionConversion<T>(
            this PropertyBuilder<IReadOnlyList<T>> builder)
        {
            return builder.HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<IReadOnlyList<T>>(v, JsonSerializerOptions.Default)!,
                new ValueComparer<IReadOnlyList<T>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v!.GetHashCode())),
                    c => c.ToList()));
        }
    }
}
