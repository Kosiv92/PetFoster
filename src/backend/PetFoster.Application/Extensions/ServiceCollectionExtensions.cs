using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Volunteers.CreateVolunteer;
using PetFoster.Application.Volunteers.UpdatePersonalInfo;
using PetFoster.Application.Volunteers.UpdateRequisites;
using PetFoster.Application.Volunteers.UpdateSocialNet;

namespace PetFoster.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<CreateVolunteerHandler>();
            services.AddTransient<UpdateVolunteerPersonalInfoHandler>();
            services.AddTransient<UpdateVolunteerSocialNetHandler>();
            services.AddTransient<UpdateVolunteerRequisitesHandler>();
            services.AddTransient<DeleteVolunteerHandler>();

            services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);            

            return services;
        }
    }
}
