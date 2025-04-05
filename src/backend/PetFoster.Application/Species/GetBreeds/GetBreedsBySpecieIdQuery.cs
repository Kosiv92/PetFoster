using PetFoster.Application.Interfaces;

namespace PetFoster.Application.Species.GetBreeds
{
    public sealed record GetBreedsBySpecieIdQuery(Guid SpecieId) : IQuery;
    
}
