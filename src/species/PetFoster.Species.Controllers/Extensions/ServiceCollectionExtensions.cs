using Microsoft.Extensions.DependencyInjection;
using PetFoster.Species.Contracts;

namespace PetFoster.Species.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpeciesContracts(this IServiceCollection services)
    {
         services.AddScoped<ISpeciesContract, SpeciesContract>();

        return services;
    }
}
