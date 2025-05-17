using PetFoster.Application.Species.AddBreed;

namespace PetFoster.WebAPI.DTO.Requests.Specie
{
    public sealed record AddBreedRequest(string Name)
    {
        public AddBreedCommand ToAddBreedCommand(Guid id)
        {
            return new AddBreedCommand(id, Name);
        }
    }
}
