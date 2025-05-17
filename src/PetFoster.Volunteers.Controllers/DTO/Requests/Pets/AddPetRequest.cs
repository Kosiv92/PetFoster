using PetFoster.Core.DTO.Volunteer;
using PetFoster.Volunteers.Application.PetManagement.AddPet;

namespace PetFoster.Volunteers.Controllers.DTO.Requests.Pets;

public sealed record AddPetRequest(string Name, string Description,
    string Health, string Coloration, CharacteristicsDto Characteristics, string OwnerPhoneNumber,
    string BirthDay, Guid Specie, Guid Breed, bool IsCastrated, bool IsVaccinated,
    AddressDto Address, string AssistanceStatus, List<AssistanceRequisitesDto> AssistanceRequisitesList);

public static class AddPetRequestExtensions
{
    public static AddPetCommand ToAddPetCommand(this AddPetRequest request, Guid id)
    {
        return new AddPetCommand(id, request.Name, request.Description, request.Health,
                    request.Coloration, request.Characteristics, request.OwnerPhoneNumber,
                    request.BirthDay, request.Specie, request.Breed, request.IsCastrated,
                    request.IsVaccinated, request.Address, request.AssistanceStatus,
                    request.AssistanceRequisitesList);
    }
}
