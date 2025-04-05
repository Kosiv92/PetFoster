using PetFoster.Application.DTO.Volunteer;

namespace PetFoster.WebAPI.DTO.Requests.Volunteer
{
    public sealed record UpdateSocialNetInfoVolunteerRequest(
        List<SocialNetContactsDto> SocialNetContactsList);    
}
