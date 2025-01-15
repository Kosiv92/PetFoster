using PetFoster.Application.DTO;
using PetFoster.Application.Volunteers.CreateVolunteer;
using PetFoster.Domain.Ids;

namespace PetFoster.WebAPI.DTO.Requests
{
    public record CreateVolunteerRequest(string FirstName, string LastName,
        string? Patronymic, string Email, string PhoneNumber, string Description, int WorkExpirience,
        List<AssistanceRequisitesDto> AssistanceRequisitesList, List<SocialNetContactsDto> SocialNetContactsList);

    public static class CreateVolunteerRequestExtensions
    {
        public static CreateVolunteerCommand ToCreateVolunteerCommand(this CreateVolunteerRequest request)
        {
            var id = VolunteerId.NewVolunteerId();

            return new CreateVolunteerCommand(id, request.FirstName, request.LastName, request.Patronymic, request.Email,
                request.PhoneNumber, request.Description, request.WorkExpirience, request.AssistanceRequisitesList, request.SocialNetContactsList);
        }
    }
}
