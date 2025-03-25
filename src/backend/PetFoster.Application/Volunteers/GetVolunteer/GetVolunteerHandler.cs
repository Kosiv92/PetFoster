using Microsoft.Extensions.Logging;
using PetFoster.Application.DTO;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.GetVolunteers;
using PetFoster.Domain.Interfaces;

namespace PetFoster.Application.Volunteers.GetVolunteer
{
    public sealed class GetVolunteerHandler : IQueryHandler<VolunteerDto, GetVolunteerQuery>
    {
        private readonly IVolunteersQueryRepository _repository;
        private readonly ILogger<GetVoluteersWithPaginationQueryHandler> _logger;

        public GetVolunteerHandler(IVolunteersQueryRepository repository, 
            ILogger<GetVoluteersWithPaginationQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public Task<VolunteerDto> Handle(GetVolunteerQuery query, CancellationToken cancellationToken = default)
        {
            return _repository.GetByIdAsync(query, cancellationToken);
        }
    }
}
