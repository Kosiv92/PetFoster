using PetFoster.Volunteers.Application.PetManagement.UpdatePetMainPhoto;

namespace PetFoster.Volunteers.Controllers.DTO.Requests.Pets;

public sealed record UpdatePetMainPhotoRequest(string FilePath)
{
    public UpdatePetMainPhotoCommand ToCommand(Guid volunteerId, Guid petId)
    {
        return new UpdatePetMainPhotoCommand(volunteerId, petId, FilePath);
    }
}
