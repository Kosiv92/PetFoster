using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using PetFoster.Infrastructure.Repositories;
using System.Text.Json;

namespace PetFoster.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string DATABASE_NAME = "Database";

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {            
            services.AddDbContext<ApplicationDbContext>(opt =>
            {                
                opt.UseNpgsql(configuration.GetConnectionString(DATABASE_NAME));
                opt.UseSnakeCaseNamingConvention();
                opt.UseLoggerFactory(CreateLoggerFactory());
            });

            services.AddScoped<IRepository<Volunteer, VolunteerId>, VolunteersRepository>();

            return services;
        }

        private static ILoggerFactory CreateLoggerFactory()
            => LoggerFactory.Create(builder => builder.AddConsole());

        public static PropertyBuilder<IReadOnlyList<T>> JsonValueObjectCollectionConversion<T>(this PropertyBuilder<IReadOnlyList<T>> builder)
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
