using PetFoster.Core.DTO.Volunteer;
using PetFoster.SharedKernel.ValueObjects.Ids;

namespace PetFoster.Volunteers.Contracts
{
    public interface IVolunteersContract
    {
        public Task<IEnumerable<PetDto>> GetPetsByBreedId(BreedId breedId, CancellationToken cancellationToken);

        public Task<IEnumerable<PetDto>> GetPetsBySpecieId(SpecieId specieId, CancellationToken cancellationToken);
    }
}
