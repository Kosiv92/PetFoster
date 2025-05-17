using PetFoster.Core;
using PetFoster.Core.DTO.Specie;
using PetFoster.Species.Application.BreedManagement.GetBreeds;
using PetFoster.Species.Application.SpecieManagement.GetSpecie;
using PetFoster.Species.Application.SpecieManagement.GetSpecies;

namespace PetFoster.Species.Application.Interfaces;

public interface ISpeciesQueryRepository
{
    public Task<PagedList<SpecieDto>> GetAllAsync(GetSpeciesWithPaginationQuery query,
        CancellationToken cancellationToken);

    public Task<SpecieDto> GetByIdAsync(GetSpecieByIdQuery query, CancellationToken cancellationToken);

    public Task<List<BreedDto>> GetBreedsBySpecieIdAsync(GetBreedsBySpecieIdQuery query,
        CancellationToken cancellationToken);
}
