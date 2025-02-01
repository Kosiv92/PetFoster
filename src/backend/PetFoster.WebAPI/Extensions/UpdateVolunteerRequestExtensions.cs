using PetFoster.Application.DTO;
using PetFoster.Application.Volunteers.UpdatePersonalInfo;
using PetFoster.Application.Volunteers.UpdateSocialNet;
using PetFoster.WebAPI.DTO.Requests.Volunteer;

namespace PetFoster.WebAPI.Extensions
{
    public static class UpdateVolunteerRequestExtensions
    {
        public static UpdateVolunteerPersonalInfoCommand ToUpdateVolunteerPersonalInfoCommand
            (this UpdateVolunteerPersonalInfoRequest request,
            Guid volunteerId) => new UpdateVolunteerPersonalInfoCommand(volunteerId,
                new FullNameDto(request.FirstName, request.LastName, request.Patronymic),
                request.Email,
                request.PhoneNumber,
                request.Description,
                request.WorkExperience);

        public static UpdateVolunteerSocialNetCommand ToUpdateVolunteerSocialNetCommand
            (this UpdateSocialNetInfoVolunteerRequest request,
            Guid volunteerId) => new UpdateVolunteerSocialNetCommand(volunteerId, request.SocialNetContactsList);
    }
}
