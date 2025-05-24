using Microsoft.Extensions.DependencyInjection;
using PetFoster.Volunteers.Contracts;

namespace PetFoster.Volunteers.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVolunteersContracts(this IServiceCollection services)
    {
        services.AddScoped<IVolunteersContract, VolunteersContract>();

        return services;
    }
}
