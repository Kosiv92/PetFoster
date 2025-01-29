using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Volunteers.CreateVolunteer;
using PetFoster.Application.Volunteers.UpdatePersonalInfo;

namespace PetFoster.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<CreateVolunteerHandler>();
            services.AddTransient<UpdatePersonalInfoHandler>();

            services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);            

            return services;
        }
    }
}
