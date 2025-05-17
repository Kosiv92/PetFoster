using PetFoster.Volunteers.Application.PetManagement.UpdatePetStatus;

namespace PetFoster.Volunteers.Controllers.DTO.Requests.Pets;

public sealed record UpdatePetStatusRequest(string AssistanseStatus)
{
    public UpdatePetStatusCommand ToCommand(Guid VolunteerId, Guid PetId)
    {
        return new UpdatePetStatusCommand(VolunteerId, PetId, AssistanseStatus);
    }
}
