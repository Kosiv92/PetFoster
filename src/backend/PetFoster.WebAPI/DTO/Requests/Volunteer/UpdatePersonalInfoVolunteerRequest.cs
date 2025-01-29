using PetFoster.Application.DTO;
using PetFoster.Application.Volunteers.UpdatePersonalInfo;

namespace PetFoster.WebAPI.DTO.Requests.Volunteer
{
    public sealed record UpdateVolunteerPersonalInfoRequest(string FirstName, string LastName,
        string? Patronymic, string Email, string PhoneNumber, string Description, int WorkExperience);

    public static class UpdateVolunteerPersonalInfoRequestExtensions
    {
        public static UpdateVolunteerPersonalInfoCommand ToUpdateVolunteerPersonalInfoCommand
            (this UpdateVolunteerPersonalInfoRequest request, 
            Guid volunteerId) => new UpdateVolunteerPersonalInfoCommand(volunteerId,
                new FullNameDto(request.FirstName, request.LastName, request.Patronymic),
                request.Email,
                request.PhoneNumber, 
                request.Description, 
                request.WorkExperience);

    }
}
