using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Volunteers.UpdatePetInfo;

namespace PetFoster.WebAPI.DTO.Requests.Volunteer
{
    public sealed record UpdatePetRequest(string Name, string Description,
        string Health, string Coloration, CharacteristicsDto Characteristics, string OwnerPhoneNumber,
        string BirthDay, Guid Specie, Guid Breed, bool IsCastrated, bool IsVaccinated,
        AddressDto Address, string AssistanceStatus, List<AssistanceRequisitesDto> AssistanceRequisitesList);

    public static class UpdatePetRequestExtensions
    {
        public static UpdatePetInfoCommand ToCommand(this UpdatePetRequest request, Guid id, Guid petId)
            => new UpdatePetInfoCommand(id, petId, request.Name, request.Description, request.Health,
                request.Coloration, request.Characteristics, request.OwnerPhoneNumber,
                request.BirthDay, request.Specie, request.Breed, request.IsCastrated,
                request.IsVaccinated, request.Address, request.AssistanceStatus,
                request.AssistanceRequisitesList);


    }
}
