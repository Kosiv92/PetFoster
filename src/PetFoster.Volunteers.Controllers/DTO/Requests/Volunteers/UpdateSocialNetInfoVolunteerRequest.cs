using PetFoster.Core.DTO.Volunteer;

namespace PetFoster.Volunteers.Controllers.DTO.Requests.Volunteers;

public sealed record UpdateSocialNetInfoVolunteerRequest(
    List<SocialNetContactsDto> SocialNetContactsList);
