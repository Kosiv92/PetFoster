using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Volunteers.UpdatePersonalInfo;
using PetFoster.Application.Volunteers.UpdateRequisites;
using PetFoster.Application.Volunteers.UpdateSocialNet;
using PetFoster.WebAPI.DTO.Requests.Volunteer;

namespace PetFoster.WebAPI.Extensions
{
    public static class UpdateVolunteerRequestExtensions
    {
        public static UpdateVolunteerPersonalInfoCommand ToUpdateVolunteerPersonalInfoCommand
            (this UpdateVolunteerPersonalInfoRequest request,
            Guid volunteerId)
        {
            return new UpdateVolunteerPersonalInfoCommand(volunteerId,
                new FullNameDto(request.FirstName, request.LastName, request.Patronymic),
                request.Email,
                request.PhoneNumber,
                request.Description,
                request.WorkExperience);
        }

        public static UpdateVolunteerSocialNetCommand ToUpdateVolunteerSocialNetCommand
            (this UpdateSocialNetInfoVolunteerRequest request,
            Guid volunteerId)
        {
            return new UpdateVolunteerSocialNetCommand(volunteerId, request.SocialNetContactsList);
        }

        public static UpdateVolunteerRequisitesCommand ToUpdateVolunteerRequisitesCommand
            (this UpdateRequisitesInfoVolunteerRequest request,
            Guid volunteerId)
        {
            return new UpdateVolunteerRequisitesCommand(volunteerId, request.AssistanceRequisitesList);
        }
    }
}
