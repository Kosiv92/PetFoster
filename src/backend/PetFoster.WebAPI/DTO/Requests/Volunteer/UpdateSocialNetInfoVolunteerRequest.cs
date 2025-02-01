using PetFoster.Application.DTO;

namespace PetFoster.WebAPI.DTO.Requests.Volunteer
{
    public sealed record UpdateSocialNetInfoVolunteerRequest(
        List<SocialNetContactsDto> SocialNetContactsList);    
}
