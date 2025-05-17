using Microsoft.Extensions.Logging;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Volunteers.Application.Interfaces;

namespace PetFoster.Volunteers.Application.PetManagement.GetPets;

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
