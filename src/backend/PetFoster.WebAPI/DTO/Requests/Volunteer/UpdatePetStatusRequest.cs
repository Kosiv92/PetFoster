using PetFoster.Application.Volunteers.UpdatePetStatus;

namespace PetFoster.WebAPI.DTO.Requests.Volunteer
{
    public sealed record UpdatePetStatusRequest(string AssistanseStatus)
    {
        public UpdatePetStatusCommand ToCommand(Guid VolunteerId, Guid PetId)
            => new UpdatePetStatusCommand(VolunteerId, PetId, AssistanseStatus);
    }
}
