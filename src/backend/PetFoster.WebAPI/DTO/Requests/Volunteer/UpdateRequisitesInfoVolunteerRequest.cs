using PetFoster.Application.DTO.Volunteer;

namespace PetFoster.WebAPI.DTO.Requests.Volunteer
{
    public sealed record UpdateRequisitesInfoVolunteerRequest(
        List<AssistanceRequisitesDto> AssistanceRequisitesList);    
}
