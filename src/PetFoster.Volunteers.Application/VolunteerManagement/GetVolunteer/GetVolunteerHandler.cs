using Microsoft.Extensions.Logging;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Volunteers.Application.Interfaces;
using PetFoster.Volunteers.Application.VolunteerManagement.GetVolunteers;

namespace PetFoster.Volunteers.Application.VolunteerManagement.GetVolunteer;

public sealed class GetVolunteerHandler : IQueryHandler<VolunteerDto, GetVolunteerByIdQuery>
{
    private readonly IVolunteersQueryRepository _repository;
    private readonly ILogger<GetVoluteersWithPaginationHandler> _logger;

    public GetVolunteerHandler(IVolunteersQueryRepository repository,
        ILogger<GetVoluteersWithPaginationHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public Task<VolunteerDto> Handle(GetVolunteerByIdQuery query, CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdAsync(query, cancellationToken);
    }
}
