using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Framework;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Species.Application.Interfaces;
using PetFoster.Species.Domain.Entities;
using PetFoster.Species.Infrastructure.DbContexts;
using PetFoster.Species.Infrastructure.Repositories;

namespace PetFoster.Species.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpeciesInfrastructureServices(this IServiceCollection services,
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

        services.AddDbContext<ReadDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString(Constants.DATABASE_NAME));
            opt.UseSnakeCaseNamingConvention();
            opt.EnableSensitiveDataLogging();
            opt.UseLoggerFactory(CreateLoggerFactory());
            opt.ConfigureWarnings(warnings => warnings.Ignore(
              RelationalEventId.PendingModelChangesWarning));
        });

        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

         services.AddScoped<IRepository<Specie, SpecieId>, SpeciesWriteRepository>();
         services.AddKeyedScoped<ISpeciesQueryRepository, SpeciesReadRepository>("ef");
         services.AddKeyedScoped<ISpeciesQueryRepository, SpeciesQueryRepository>("dapper");
         services.AddKeyedScoped<IUnitOfWork, UnitOfWork>("species");

        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        return services;
    }

    private static ILoggerFactory CreateLoggerFactory()
    {
        return LoggerFactory.Create(builder => builder.AddConsole());
    }


}
