using PetFoster.Application.DTO;
using PetFoster.Application.Volunteers.CreateVolunteer;
using PetFoster.Domain.Ids;

namespace PetFoster.WebAPI.DTO.Requests.Volunteer
{
    public record UpdateSocialNetInfoVolunteerRequest(
        List<SocialNetContactsDto> SocialNetContactsList);    
}
