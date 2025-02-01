using PetFoster.Application.DTO;

namespace PetFoster.WebAPI.DTO.Requests.Volunteer
{
    public sealed record UpdateRequisitesInfoVolunteerRequest(
        List<AssistanceRequisitesDto> AssistanceRequisitesList);    
}
