using PetFoster.Core.DTO.Volunteer;

namespace PetFoster.Volunteers.Contracts.DTO.Requests.Pets;

public sealed record AddPetRequest(string Name, string Description,
    string Health, string Coloration, CharacteristicsDto Characteristics, string OwnerPhoneNumber,
    string BirthDay, Guid Specie, Guid Breed, bool IsCastrated, bool IsVaccinated,
    AddressDto Address, string AssistanceStatus, List<AssistanceRequisitesDto> AssistanceRequisitesList);