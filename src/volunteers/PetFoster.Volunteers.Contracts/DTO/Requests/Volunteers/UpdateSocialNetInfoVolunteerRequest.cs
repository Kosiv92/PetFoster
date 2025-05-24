using PetFoster.Core.DTO.Volunteer;

namespace PetFoster.Volunteers.Contracts.DTO.Requests.Volunteers;

public sealed record UpdateSocialNetInfoVolunteerRequest(
    List<SocialNetContactsDto> SocialNetContactsList);
