using Microsoft.Extensions.Logging;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.GetVolunteers;
using PetFoster.Domain.Interfaces;

namespace PetFoster.Application.Volunteers.GetVolunteer
{
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
}
