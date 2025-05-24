using PetFoster.Core.DTO.Volunteer;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Volunteers.Application.Interfaces;
using PetFoster.Volunteers.Application.PetManagement.GetPetsByBreedId;
using PetFoster.Volunteers.Application.PetManagement.GetPetsBySpecieId;
using PetFoster.Volunteers.Contracts;

namespace PetFoster.Volunteers.Presentation;

public class VolunteersContract : IVolunteersContract
{
    private readonly IVolunteersQueryRepository _volunteersQueryRepository;

    public VolunteersContract(IVolunteersQueryRepository volunteersQueryRepository)
    {
        _volunteersQueryRepository = volunteersQueryRepository;
    }

    public Task<IEnumerable<PetDto>> GetPetsByBreedId(BreedId breedId, CancellationToken cancellationToken)
    {
        return _volunteersQueryRepository.GetPetsByBreedId(new GetPetsByBreedIdQuery(breedId), cancellationToken);
    }

    public Task<IEnumerable<PetDto>> GetPetsBySpecieId(SpecieId specieId, CancellationToken cancellationToken)
    {
        return _volunteersQueryRepository.GetPetsBySpecieId(new GetPetsBySpecieIdQuery(specieId), cancellationToken);
    }
}
