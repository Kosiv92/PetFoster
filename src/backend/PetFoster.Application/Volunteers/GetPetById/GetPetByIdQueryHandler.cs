using Microsoft.Extensions.Logging;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Interfaces;
using PetFoster.Domain.Interfaces;

namespace PetFoster.Application.Volunteers.GetPetByID
{
    public sealed class GetPetByIdQueryHandler 
        : IQueryHandler<PetDto, GetPetByIdQuery>
    {
        private readonly IVolunteersQueryRepository _repository;
        private readonly ILogger<GetPetByIdQueryHandler> _logger;

        public GetPetByIdQueryHandler(IVolunteersQueryRepository repository, 
            ILogger<GetPetByIdQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PetDto> Handle(GetPetByIdQuery query,
            CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetPetByIdAsync(query, cancellationToken);
            if(result != null)
            {
                _logger.LogInformation("Successfully received pet with id {PetId} in volunteer with id {VolunteerId}", 
                    query.PetId, query.VolunteerId);
            }
            else
            {
                _logger.LogWarning("Not found pet with id {PetId} in volunteer with id {VolunteerId}",
                    query.PetId, query.VolunteerId);
            }
            return result;
        }
    }
}
