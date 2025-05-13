using Microsoft.Extensions.Logging;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Interfaces;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;

namespace PetFoster.Application.Volunteers.GetPets
{
    public sealed class GetPetsWithPaginationHandler 
        : IQueryHandler<PagedList<PetDto>, GetPetsWithPaginationQuery>
    {
        private readonly IVolunteersQueryRepository _repository;
        private readonly ILogger<GetPetsWithPaginationHandler> _logger;

        public GetPetsWithPaginationHandler(IVolunteersQueryRepository repository, 
            ILogger<GetPetsWithPaginationHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public Task<PagedList<PetDto>> Handle(
            GetPetsWithPaginationQuery query, 
            CancellationToken cancellationToken = default)
        {
            return _repository.GetPetWithPaginationAsync(query, cancellationToken);
        }
    }
}
