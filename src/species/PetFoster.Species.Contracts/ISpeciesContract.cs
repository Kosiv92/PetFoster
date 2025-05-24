using PetFoster.Core.DTO.Specie;
using PetFoster.SharedKernel.ValueObjects.Ids;

namespace PetFoster.Species.Contracts;

public interface ISpeciesContract
{
    Task<SpecieDto> GetSpecieById(SpecieId specieId, 
        CancellationToken cancellationToken = default);
}
