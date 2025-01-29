using PetFoster.Application.DTO;
using PetFoster.Application.Volunteers.CreateVolunteer;
using PetFoster.Domain.Ids;

namespace PetFoster.WebAPI.DTO.Requests.Volunteer
{
    public record CreateVolunteerRequest(string FirstName, string LastName,
        string? Patronymic, string Email, string PhoneNumber, string Description, int WorkExperience,
        List<AssistanceRequisitesDto> AssistanceRequisitesList, List<SocialNetContactsDto> SocialNetContactsList);

    public static class CreateVolunteerRequestExtensions
    {
        public static CreateVolunteerCommand ToCreateVolunteerCommand(this CreateVolunteerRequest request)
            => new CreateVolunteerCommand(VolunteerId.NewVolunteerId(),
                new FullNameDto(request.FirstName, request.LastName, request.Patronymic),
                request.Email,
                request.PhoneNumber,
                request.Description,
                request.WorkExperience,
                request.AssistanceRequisitesList,
                request.SocialNetContactsList);

    }
}
