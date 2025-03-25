﻿using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Interfaces;

namespace PetFoster.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddCommands();

            services.AddQueries();

            services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);            

            return services;
        }

        private static IServiceCollection AddCommands(this IServiceCollection services)
        {
            return services.Scan(scan => scan.FromAssemblies(typeof(ServiceCollectionExtensions).Assembly)
                .AddClasses(classes => classes
                    .AssignableToAny(typeof(ICommandHandler<,>), typeof(ICommandHandler<>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime());
        }

        private static IServiceCollection AddQueries(this IServiceCollection services)
        {
            return services.Scan(scan => scan.FromAssemblies(typeof(ServiceCollectionExtensions).Assembly)
                .AddClasses(classes => classes
                    .AssignableTo(typeof(IQueryHandler<,>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime());
        }
    }
}
