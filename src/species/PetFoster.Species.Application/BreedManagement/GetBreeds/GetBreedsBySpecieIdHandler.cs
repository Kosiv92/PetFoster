using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Specie;
using PetFoster.Species.Application.Interfaces;

namespace PetFoster.Species.Application.BreedManagement.GetBreeds;

public sealed class GetBreedsBySpecieIdHandler
    : IQueryHandler<List<BreedDto>, GetBreedsBySpecieIdQuery>
{
    private readonly ISpeciesQueryRepository _repository;
    private readonly ILogger<GetBreedsBySpecieIdHandler> _logger;

    public GetBreedsBySpecieIdHandler(
        [FromKeyedServices("dapper")] ISpeciesQueryRepository repository,
        ILogger<GetBreedsBySpecieIdHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<BreedDto>> Handle(GetBreedsBySpecieIdQuery query,
        CancellationToken cancellationToken = default)
    {
        List<BreedDto>? breeds = await _repository
            .GetBreedsBySpecieIdAsync(query, cancellationToken);


        _logger.LogInformation("Get {BreedsCount} breeds with query {@query}",
            breeds?.Count, query);

        return breeds;
    }
}
