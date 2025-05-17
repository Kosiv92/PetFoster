using PetFoster.Core.DTO.Volunteer;
using PetFoster.Volunteers.Application.PetManagement.UpdatePetInfo;

namespace PetFoster.Volunteers.Controllers.DTO.Requests.Pets;

public sealed record UpdatePetRequest(string Name, string Description,
    string Health, string Coloration, CharacteristicsDto Characteristics, string OwnerPhoneNumber,
    string BirthDay, Guid Specie, Guid Breed, bool IsCastrated, bool IsVaccinated,
    AddressDto Address, string AssistanceStatus, List<AssistanceRequisitesDto> AssistanceRequisitesList);

public static class UpdatePetRequestExtensions
{
    public static UpdatePetInfoCommand ToCommand(this UpdatePetRequest request, Guid id, Guid petId)
    {
        return new UpdatePetInfoCommand(id, petId, request.Name, request.Description, request.Health,
                    request.Coloration, request.Characteristics, request.OwnerPhoneNumber,
                    request.BirthDay, request.Specie, request.Breed, request.IsCastrated,
                    request.IsVaccinated, request.Address, request.AssistanceStatus,
                    request.AssistanceRequisitesList);
    }
}
