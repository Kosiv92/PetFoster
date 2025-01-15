using PetFoster.Application.DTO;
using PetFoster.Domain.Ids;

namespace PetFoster.Application.Volunteers.CreateVolunteer
{
    public record CreateVolunteerCommand(VolunteerId id, string FirstName, string LastName,
        string? Patronymic, string Email, string PhoneNumber, string Description, int WorkExpirience,
        List<AssistanceRequisitesDto> AssistanceRequisitesList, List<SocialNetContactsDto> SocialNetContactsList);
}
