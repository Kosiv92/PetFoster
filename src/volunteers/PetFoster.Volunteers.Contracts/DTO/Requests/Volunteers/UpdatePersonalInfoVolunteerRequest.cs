namespace PetFoster.Volunteers.Contracts.DTO.Requests.Volunteers;

public sealed record UpdateVolunteerPersonalInfoRequest(string FirstName, string LastName,
    string? Patronymic, string Email, string PhoneNumber, string Description, int WorkExperience);
