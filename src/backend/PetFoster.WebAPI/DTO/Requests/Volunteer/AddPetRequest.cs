using PetFoster.Application.DTO;
using PetFoster.Application.Volunteers.AddPet;

namespace PetFoster.WebAPI.DTO.Requests.Volunteer
{
    public sealed record AddPetRequest(string Name, string Description, 
        string Health, string Coloration, CharacteristicsDto Characteristics, string OwnerPhoneNumber,
        string BirthDay, string Specie, string Breed, bool IsCastrated, bool IsVaccinated, 
        AddressDto Address, string AssistanceStatus, List<AssistanceRequisitesDto> AssistanceRequisitesList, 
        IFormFileCollection Files);

    public static class AddPetRequestExtensions
    {
        public static AddPetCommand ToAddPetCommand(this AddPetRequest request, Guid id, IEnumerable<CreateFileDto> files)
            => new AddPetCommand(id, request.Name, request.Description, request.Health, 
                request.Coloration, request.Characteristics, request.OwnerPhoneNumber, 
                request.BirthDay, request.Specie, request.Breed, request.IsCastrated, 
                request.IsVaccinated, request.Address, request.AssistanceStatus, 
                request.AssistanceRequisitesList, files);

          
    }
}
