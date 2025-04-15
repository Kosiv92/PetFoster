using PetFoster.Application.Volunteers.UpdatePetMainPhoto;

namespace PetFoster.WebAPI.DTO.Requests.Volunteer
{
    public sealed record UpdatePetMainPhotoRequest(string FilePath)
    {
        public UpdatePetMainPhotoCommand ToCommand(Guid volunteerId, Guid petId)
        {
            return new UpdatePetMainPhotoCommand(volunteerId, petId, FilePath);
        }
    }
}
