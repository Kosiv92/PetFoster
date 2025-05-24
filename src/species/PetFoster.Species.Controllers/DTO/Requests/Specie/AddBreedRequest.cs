using PetFoster.Species.Application.BreedManagement.AddBreed;

namespace PetFoster.Species.Presentation.DTO.Requests.Specie;

public sealed record AddBreedRequest(string Name)
{
    public AddBreedCommand ToAddBreedCommand(Guid id)
    {
        return new AddBreedCommand(id, Name);
    }
}
