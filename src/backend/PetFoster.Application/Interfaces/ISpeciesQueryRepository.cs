using PetFoster.Application.DTO.Specie;
using PetFoster.Application.Species.GetBreeds;
using PetFoster.Application.Species.GetSpecie;
using PetFoster.Application.Species.GetSpecies;
using PetFoster.Domain.Shared;

namespace PetFoster.Application.Interfaces
{
    public interface ISpeciesQueryRepository
    {
        public Task<PagedList<SpecieDto>> GetAllAsync(GetSpeciesWithPaginationQuery query,
            CancellationToken cancellationToken);

        public Task<SpecieDto> GetByIdAsync(GetSpecieByIdQuery query, CancellationToken cancellationToken);

        public Task<List<BreedDto>> GetBreedsBySpecieIdAsync(GetBreedsBySpecieIdQuery query,
            CancellationToken cancellationToken);
    }
}
