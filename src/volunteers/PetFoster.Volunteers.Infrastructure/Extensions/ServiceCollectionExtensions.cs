using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minio;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Messaging;
using PetFoster.Framework;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Volunteers.Application.Interfaces;
using PetFoster.Volunteers.Domain.Entities;
using PetFoster.Volunteers.Infrastructure.BackgroundServices;
using PetFoster.Volunteers.Infrastructure.DbContexts;
using PetFoster.Volunteers.Infrastructure.Files;
using PetFoster.Volunteers.Infrastructure.MessageQueues;
using PetFoster.Volunteers.Infrastructure.Options;
using PetFoster.Volunteers.Infrastructure.Providers;
using PetFoster.Volunteers.Infrastructure.Repositories;
using System.Text.Json;

namespace PetFoster.Volunteers.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVolunteerInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<WriteDbContext>(opt =>
       {
            opt.UseNpgsql(configuration.GetConnectionString(Constants.DATABASE_NAME));
            opt.UseSnakeCaseNamingConvention();
            opt.EnableSensitiveDataLogging();
            opt.UseLoggerFactory(CreateLoggerFactory());
            opt.ConfigureWarnings(warnings => warnings.Ignore(
              RelationalEventId.PendingModelChangesWarning));
       });

        services.AddMinioServices(configuration);

         services.AddHostedService<FilesCleanerBackgroundService>();

         services.AddSingleton<IMessageQueue<IEnumerable<Application.Files.FileInfo>>,
                       InMemoryMessageQueue<IEnumerable<Application.Files.FileInfo>>>();
         services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

         services.AddScoped<IRepository<Volunteer, VolunteerId>, VolunteersRepository>();
         services.AddScoped<IVolunteersQueryRepository, VolunteersQueryRepository>();
         services.AddKeyedScoped<IUnitOfWork, UnitOfWork>("volunteers");

         services.AddTransient<IFilesCleanerService, FilesCleanerService>();

        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        return services;
    }

    private static IServiceCollection AddMinioServices(this IServiceCollection services,
        IConfiguration configuration)
    {
         services.AddMinio(options =>
       {
           MinioOptions minioOptions = configuration.GetSection(MinioOptions.MINIO).Get<MinioOptions>()
           ?? throw new ApplicationException("Missing minio configuration");

            options.WithEndpoint(minioOptions.Endpoint);
            options.WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey);
            options.WithSSL(minioOptions.WithSsl);
       });

         services.AddScoped<IFileProvider, MinioProvider>();

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
