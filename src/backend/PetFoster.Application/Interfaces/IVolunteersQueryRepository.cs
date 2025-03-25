using PetFoster.Application.DTO;
using PetFoster.Application.Volunteers.GetVolunteer;
using PetFoster.Application.Volunteers.GetVolunteers;
using PetFoster.Domain.Shared;

namespace PetFoster.Domain.Interfaces
{
    public interface IVolunteersQueryRepository
    {
        public Task<PagedList<VolunteerDto>> GetAllAsync(GetVoluteersWithPaginationQuery query, 
            CancellationToken cancellationToken);

        public Task<VolunteerDto> GetByIdAsync(GetVolunteerQuery query, CancellationToken cancellationToken);
    }
}
