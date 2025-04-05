using PetFoster.Application.Species.CreateSpecie;

namespace PetFoster.WebAPI.DTO.Requests.Specie
{
    public sealed record CreateSpecieRequest(string Name)
    {
        public CreateSpecieCommand ToCreateSpecieCommand(Guid id) 
            => new CreateSpecieCommand(id, Name);
    }    
}
