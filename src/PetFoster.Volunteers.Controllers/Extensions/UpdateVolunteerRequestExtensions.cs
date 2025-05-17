using PetFoster.Core.DTO.Volunteer;
using PetFoster.Volunteers.Application.VolunteerManagement.UpdatePersonalInfo;
using PetFoster.Volunteers.Application.VolunteerManagement.UpdateRequisites;
using PetFoster.Volunteers.Application.VolunteerManagement.UpdateSocialNet;
using PetFoster.Volunteers.Controllers.DTO.Requests.Volunteers;

namespace PetFoster.Volunteers.Controllers.Extensions;

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
