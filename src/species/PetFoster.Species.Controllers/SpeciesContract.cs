using Microsoft.Extensions.DependencyInjection;
using PetFoster.Core.DTO.Specie;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Species.Application.Interfaces;
using PetFoster.Species.Application.SpecieManagement.GetSpecie;
using PetFoster.Species.Contracts;

namespace PetFoster.Species.Presentation;

public class SpeciesContract : ISpeciesContract
{
    private readonly ISpeciesQueryRepository _speciesQueryRepository;

    public SpeciesContract([FromKeyedServices("dapper")] ISpeciesQueryRepository speciesQueryRepository)
    {
        _speciesQueryRepository = speciesQueryRepository;
    }

    public Task<SpecieDto> GetSpecieById(SpecieId specieId, CancellationToken cancellationToken = default)
    {
        return _speciesQueryRepository.GetByIdAsync(new GetSpecieByIdQuery(specieId), cancellationToken);
    }
}
