using PetFoster.Core.DTO.Volunteer;

namespace PetFoster.Volunteers.Contracts.DTO.Requests.Volunteers;

public sealed record CreateVolunteerRequest(string FirstName, string LastName,
    string? Patronymic, string Email, string PhoneNumber, string Description, int WorkExperience,
    List<AssistanceRequisitesDto> AssistanceRequisitesList, List<SocialNetContactsDto> SocialNetContactsList);
