using PetFoster.Core.DTO.Volunteer;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Volunteers.Application.PetManagement.AddPet;
using PetFoster.Volunteers.Application.PetManagement.GetPets;
using PetFoster.Volunteers.Application.PetManagement.UpdatePetInfo;
using PetFoster.Volunteers.Application.PetManagement.UpdatePetMainPhoto;
using PetFoster.Volunteers.Application.PetManagement.UpdatePetStatus;
using PetFoster.Volunteers.Application.VolunteerManagement.CreateVolunteer;
using PetFoster.Volunteers.Application.VolunteerManagement.GetVolunteers;
using PetFoster.Volunteers.Application.VolunteerManagement.UpdatePersonalInfo;
using PetFoster.Volunteers.Application.VolunteerManagement.UpdateRequisites;
using PetFoster.Volunteers.Application.VolunteerManagement.UpdateSocialNet;
using PetFoster.Volunteers.Contracts.DTO.Requests.Pets;
using PetFoster.Volunteers.Contracts.DTO.Requests.Volunteers;

namespace PetFoster.Volunteers.Presentation.Extensions;

public static class VolunteerRequestExtensions
{
    public static UpdateVolunteerPersonalInfoCommand ToUpdateVolunteerPersonalInfoCommand
        (this UpdateVolunteerPersonalInfoRequest request,
        Guid volunteerId)
    {
        return new UpdateVolunteerPersonalInfoCommand(volunteerId,
            new FullNameDto(request.FirstName, request.LastName, request.Patronymic),
            request.Email,
            request.PhoneNumber,
            request.Description,
            request.WorkExperience);
    }

    public static UpdateVolunteerSocialNetCommand ToUpdateVolunteerSocialNetCommand
        (this UpdateSocialNetInfoVolunteerRequest request,
        Guid volunteerId)
    {
        return new UpdateVolunteerSocialNetCommand(volunteerId, request.SocialNetContactsList);
    }

    public static UpdateVolunteerRequisitesCommand ToUpdateVolunteerRequisitesCommand
        (this UpdateRequisitesInfoVolunteerRequest request,
        Guid volunteerId)
    {
        return new UpdateVolunteerRequisitesCommand(volunteerId, request.AssistanceRequisitesList);
    }

    public static AddPetCommand ToAddPetCommand(this AddPetRequest request, Guid id)
    {
        return new AddPetCommand(id, request.Name, request.Description, request.Health,
                    request.Coloration, request.Characteristics, request.OwnerPhoneNumber,
                    request.BirthDay, request.Specie, request.Breed, request.IsCastrated,
                    request.IsVaccinated, request.Address, request.AssistanceStatus,
                    request.AssistanceRequisitesList);
    }

    public static GetPetsWithPaginationQuery ToGetPetsWithPaginationQuery(this GetPetsWithPagiationRequest request)
    {
        return new GetPetsWithPaginationQuery(
            request.Page,
            request.PageSize,
            request.SortBy,
            request.SortAsc,
            request.FilterList);
    }

    public static UpdatePetMainPhotoCommand ToUpdatePetMainPhotoCommand(this UpdatePetMainPhotoRequest request,
        Guid volunteerId, Guid petId)
    {
        return new UpdatePetMainPhotoCommand(volunteerId, petId, request.FilePath);
    }

    public static UpdatePetInfoCommand ToUpdatePetInfoCommand(this UpdatePetRequest request, Guid id, Guid petId)
    {
        return new UpdatePetInfoCommand(id, petId, request.Name, request.Description, request.Health,
                    request.Coloration, request.Characteristics, request.OwnerPhoneNumber,
                    request.BirthDay, request.Specie, request.Breed, request.IsCastrated,
                    request.IsVaccinated, request.Address, request.AssistanceStatus,
                    request.AssistanceRequisitesList);
    }

    public static UpdatePetStatusCommand ToUpdatePetStatusCommand(this UpdatePetStatusRequest request,
        Guid VolunteerId, Guid PetId)
    {
        return new UpdatePetStatusCommand(VolunteerId, PetId, request.AssistanseStatus);
    }

    public static CreateVolunteerCommand ToCreateVolunteerCommand(this CreateVolunteerRequest request)
    {
        return new CreateVolunteerCommand(VolunteerId.NewVolunteerId(),
                    new FullNameDto(request.FirstName, request.LastName, request.Patronymic),
                    request.Email,
                    request.PhoneNumber,
                    request.Description,
                    request.WorkExperience,
                    request.AssistanceRequisitesList,
                    request.SocialNetContactsList);
    }


    public static GetVoluteersWithPaginationQuery ToGetVoluteersWithPaginationQuery(this GetVolunteersWithPagiationRequest request)
    {
        return new(request.PositionFrom, request.PositionTo, request.Page, request.PageSize);
    }



}
