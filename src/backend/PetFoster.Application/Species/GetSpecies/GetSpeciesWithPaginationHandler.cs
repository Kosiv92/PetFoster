using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFoster.Application.DTO.Specie;
using PetFoster.Application.Interfaces;
using PetFoster.Domain.Shared;

namespace PetFoster.Application.Species.GetSpecies
{
    public sealed class GetSpeciesWithPaginationHandler 
        : IQueryHandler<PagedList<SpecieDto>, GetSpeciesWithPaginationQuery>
    {
        private readonly ISpeciesQueryRepository _repository;
        private readonly ILogger<GetSpeciesWithPaginationHandler> _logger;

        public GetSpeciesWithPaginationHandler([FromKeyedServices("dapper")] ISpeciesQueryRepository repository, 
            ILogger<GetSpeciesWithPaginationHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PagedList<SpecieDto>> Handle(GetSpeciesWithPaginationQuery query, 
            CancellationToken cancellationToken = default)
        {
            var species = await _repository.GetAllAsync(query, cancellationToken);

            _logger.LogInformation("Get {SpeciesCount} species with query {@query}",
                species?.Items.Count, query);

            return species;
        }
    }
}
