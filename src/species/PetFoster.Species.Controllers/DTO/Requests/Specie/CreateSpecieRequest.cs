using PetFoster.Species.Application.SpecieManagement.CreateSpecie;

namespace PetFoster.Species.Presentation.DTO.Requests.Specie;

public sealed record CreateSpecieRequest(string Name)
{
    public CreateSpecieCommand ToCreateSpecieCommand(Guid id)
    {
        return new CreateSpecieCommand(id, Name);
    }
}
