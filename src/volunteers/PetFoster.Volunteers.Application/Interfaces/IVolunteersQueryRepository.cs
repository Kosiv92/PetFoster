using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.Models;
using PetFoster.Volunteers.Application.PetManagement.GetPetById;
using PetFoster.Volunteers.Application.PetManagement.GetPets;
using PetFoster.Volunteers.Application.PetManagement.GetPetsByBreedId;
using PetFoster.Volunteers.Application.PetManagement.GetPetsBySpecieId;
using PetFoster.Volunteers.Application.PetManagement.GetPetsByVolunteerId;
using PetFoster.Volunteers.Application.VolunteerManagement.GetVolunteer;
using PetFoster.Volunteers.Application.VolunteerManagement.GetVolunteers;

namespace PetFoster.Volunteers.Application.Interfaces;

public interface IVolunteersQueryRepository
{
    public Task<PagedList<VolunteerDto>> GetAllAsync(GetVoluteersWithPaginationQuery query,
        CancellationToken cancellationToken);

    public Task<VolunteerDto> GetByIdAsync(GetVolunteerByIdQuery query,
        CancellationToken cancellationToken);

    public Task<IEnumerable<PetDto>> GetPetsBySpecieId(GetPetsBySpecieIdQuery query,
        CancellationToken cancellationToken);

    public Task<IEnumerable<PetDto>> GetPetsByBreedId(GetPetsByBreedIdQuery query,
        CancellationToken cancellationToken);

    public Task<IEnumerable<PetDto>> GetPetsByVolunteerId(GetPetsByVolunteerIdQuery query,
        CancellationToken cancellationToken);

    public Task<PetDto> GetPetByIdAsync(GetPetByIdQuery query,
        CancellationToken cancellationToken);

    public Task<PagedList<PetDto>> GetPetWithPaginationAsync(GetPetsWithPaginationQuery query,
        CancellationToken cancellationToken);
}
