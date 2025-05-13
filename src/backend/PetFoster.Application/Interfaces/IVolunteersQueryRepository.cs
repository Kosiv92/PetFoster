using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Volunteers.GetPetByID;
using PetFoster.Application.Volunteers.GetPets;
using PetFoster.Application.Volunteers.GetPetsByBreedId;
using PetFoster.Application.Volunteers.GetPetsBySpecieId;
using PetFoster.Application.Volunteers.GetPetsByVolunteerId;
using PetFoster.Application.Volunteers.GetVolunteer;
using PetFoster.Application.Volunteers.GetVolunteers;
using PetFoster.Domain.Shared;

namespace PetFoster.Domain.Interfaces
{
    public interface IVolunteersQueryRepository
    {
        public Task<PagedList<VolunteerDto>> GetAllAsync(GetVoluteersWithPaginationQuery query, 
            CancellationToken cancellationToken);

        public Task<VolunteerDto> GetByIdAsync(GetVolunteerByIdQuery query, 
            CancellationToken cancellationToken);

        public Task<PetDto> GetPetByIdAsync(GetPetByIdQuery query,
            CancellationToken cancellationToken);

        public Task<PagedList<PetDto>> GetPetWithPaginationAsync(GetPetsWithPaginationQuery query,
            CancellationToken cancellationToken);

        public Task<IEnumerable<PetDto>> GetPetsBySpecieId(GetPetsBySpecieIdQuery query, 
            CancellationToken cancellationToken);

        public Task<IEnumerable<PetDto>> GetPetsByBreedId(GetPetsByBreedIdQuery query,
            CancellationToken cancellationToken);

        public Task<IEnumerable<PetDto>> GetPetsByVolunteerId(GetPetsByVolunteerIdQuery query, 
            CancellationToken cancellationToken);
    }
}
