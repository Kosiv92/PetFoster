using PetFoster.Core.DTO.Volunteer;
using PetFoster.Volunteers.Application.VolunteerManagement.CreateVolunteer;
using PetFoster.Volunteers.Domain.Ids;

namespace PetFoster.Volunteers.Controllers.DTO.Requests.Volunteers;

public sealed record CreateVolunteerRequest(string FirstName, string LastName,
    string? Patronymic, string Email, string PhoneNumber, string Description, int WorkExperience,
    List<AssistanceRequisitesDto> AssistanceRequisitesList, List<SocialNetContactsDto> SocialNetContactsList);

public static class CreateVolunteerRequestExtensions
{
    public static CreateVolunteerCommand ToCreateVolunteerCommand(this CreateVolunteerRequest request)
    {
        return new CreateVolunteerCommand(VolunteerId.NewVolunteerId(),
                    new FullNameDto(request.FirstName, request.LastName, request.Patronymic),
                    request.Email,
                    request.PhoneNumber,
                    request.Description,
                    request.WorkExperience,
                    request.AssistanceRequisitesList,
                    request.SocialNetContactsList);
    }
}
