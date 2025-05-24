using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.Models;
using PetFoster.SharedKernel;
using PetFoster.Framework;
using PetFoster.Volunteers.Application.PetManagement.AddPet;
using PetFoster.Volunteers.Application.PetManagement.DeletePet;
using PetFoster.Volunteers.Application.PetManagement.GetPetById;
using PetFoster.Volunteers.Application.PetManagement.GetPets;
using PetFoster.Volunteers.Application.PetManagement.UpdatePetInfo;
using PetFoster.Volunteers.Application.PetManagement.UpdatePetMainPhoto;
using PetFoster.Volunteers.Application.PetManagement.UpdatePetStatus;
using PetFoster.Volunteers.Application.PetManagement.UploadFilesToPet;
using PetFoster.Volunteers.Application.VolunteerManagement.CreateVolunteer;
using PetFoster.Volunteers.Application.VolunteerManagement.DeleteVolunteer;
using PetFoster.Volunteers.Application.VolunteerManagement.GetVolunteer;
using PetFoster.Volunteers.Application.VolunteerManagement.GetVolunteers;
using PetFoster.Volunteers.Application.VolunteerManagement.UpdatePersonalInfo;
using PetFoster.Volunteers.Application.VolunteerManagement.UpdateRequisites;
using PetFoster.Volunteers.Application.VolunteerManagement.UpdateSocialNet;
using PetFoster.Volunteers.Contracts.DTO.Requests.Pets;
using PetFoster.Volunteers.Contracts.DTO.Requests.Volunteers;
using PetFoster.Volunteers.Presentation.Extensions;
using PetFoster.Volunteers.Presentation.Processors;

namespace PetFoster.Volunteers.Presentation;

[ApiController]
[Route("[controller]")]
public class VolunteerController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetVolunteers([FromQuery] GetVolunteersWithPagiationRequest request,
        [FromServices] GetVoluteersWithPaginationHandler handler,
        CancellationToken cancellationToken = default)
    {
        GetVoluteersWithPaginationQuery query = request.ToGetVoluteersWithPaginationQuery();

        PagedList<VolunteerDto> volunteerDtos = await handler.Handle(query, cancellationToken);

        return Ok(volunteerDtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetVolunteer([FromRoute] Guid id,
        [FromServices] GetVolunteerHandler handler,
        CancellationToken cancellationToken = default)
    {
        GetVolunteerByIdQuery query = new(id);

        VolunteerDto volunteerDto = await handler.Handle(query, cancellationToken);

        return Ok(volunteerDto);
    }


    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromServices] CreateVolunteerHandler handler,
        [FromBody] CreateVolunteerRequest request, CancellationToken cancellationToken = default)
    {
        CreateVolunteerCommand command = request.ToCreateVolunteerCommand();

        CSharpFunctionalExtensions.Result<Guid, ErrorList> result = await handler.Handle(command, cancellationToken);

        return result.IsFailure ? (ActionResult<Guid>)result.Error.ToResponse() : (ActionResult<Guid>)Ok(result.Value);
    }

    [HttpPut("{id:guid}/personal-info")]
    public async Task<IActionResult> UpdatePersonalInfo([FromRoute] Guid id,
        [FromServices] UpdateVolunteerPersonalInfoHandler handler,
        [FromBody] UpdateVolunteerPersonalInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        UpdateVolunteerPersonalInfoCommand command = request.ToUpdateVolunteerPersonalInfoCommand(id);

        CSharpFunctionalExtensions.Result<Guid, ErrorList> result = await handler.Handle(command, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : (IActionResult)Ok(result.Value);
    }

    [HttpPut("{id:guid}/social-net")]
    public async Task<IActionResult> UpdateSocialNetInfo([FromRoute] Guid id,
        [FromServices] UpdateVolunteerSocialNetHandler handler,
        [FromBody] UpdateSocialNetInfoVolunteerRequest request,
        CancellationToken cancellationToken = default)
    {
        UpdateVolunteerSocialNetCommand command = request.ToUpdateVolunteerSocialNetCommand(id);

        CSharpFunctionalExtensions.Result<Guid, ErrorList> result = await handler.Handle(command, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : (IActionResult)Ok(result.Value);
    }

    [HttpPut("{id:guid}/requisites")]
    public async Task<IActionResult> UpdateRequisites([FromRoute] Guid id,
        [FromServices] UpdateVolunteerRequisitesHandler handler,
        [FromBody] UpdateRequisitesInfoVolunteerRequest request,
        CancellationToken cancellationToken = default)
    {
        UpdateVolunteerRequisitesCommand command = request.ToUpdateVolunteerRequisitesCommand(id);

        CSharpFunctionalExtensions.Result<Guid, ErrorList> result = await handler.Handle(command, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : (IActionResult)Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id,
        [FromServices] DeleteVolunteerHandler handler,
        CancellationToken cancellationToken = default)
    {
        DeleteVolunteerCommand command = new(id);

        CSharpFunctionalExtensions.Result<Guid, ErrorList> result = await handler.Handle(command, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpPost("{id:guid}/pet")]
    public async Task<ActionResult> AddPet([FromRoute] Guid id,
        [FromBody] AddPetRequest request,
        [FromServices] AddPetHandler handler,
        CancellationToken cancellationToken = default)
    {
        AddPetCommand command = request.ToAddPetCommand(id);

        CSharpFunctionalExtensions.Result<Guid, ErrorList> result = await handler.Handle(command, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpGet("{id:guid}/pet")]
    public async Task<ActionResult> GetPets([FromRoute] Guid id,
        [FromQuery] GetPetsWithPagiationRequest request,
        [FromServices] GetPetsWithPaginationHandler handler,
        CancellationToken cancellationToken = default)
    {
        GetPetsWithPaginationQuery query = request.ToGetPetsWithPaginationQuery();
        PagedList<PetDto> petDto = await handler.Handle(query, cancellationToken);

        return Ok(petDto);
    }

    [HttpGet("{id:guid}/pet/{petId:guid}")]
    public async Task<ActionResult> GetPet([FromRoute] Guid id, [FromRoute] Guid petId,
        [FromServices] GetPetByIdQueryHandler handler,
        CancellationToken cancellationToken = default)
    {
        GetPetByIdQuery query = new(id, petId);
        PetDto petDto = await handler.Handle(query, cancellationToken);

        return Ok(petDto);
    }

    [HttpPut("{id:guid}/pet/{petId:guid}/status")]
    public async Task<ActionResult> UpdatePetStatus([FromRoute] Guid id, [FromRoute] Guid petId,
        [FromBody] UpdatePetStatusRequest request,
        [FromServices] UpdatePetStatusHandler handler,
        CancellationToken cancellationToken = default)
    {
        UpdatePetStatusCommand command = request.ToUpdatePetStatusCommand(id, petId);
        CSharpFunctionalExtensions.Result<Guid, ErrorList> result = await handler.Handle(command, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpPut("{id:guid}/pet/{petId:guid}")]
    public async Task<ActionResult> UpdatePet([FromRoute] Guid id, [FromRoute] Guid petId,
        [FromBody] UpdatePetRequest request,
        [FromServices] UpdatePetInfoHandler handler,
        CancellationToken cancellationToken = default)
    {
        UpdatePetInfoCommand command = request.ToUpdatePetInfoCommand(id, petId);
        CSharpFunctionalExtensions.Result<Guid, ErrorList> result = await handler.Handle(command, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpDelete("{id:guid}/pet/{petId:guid}")]
    public async Task<ActionResult> DeletePet([FromRoute] Guid id, [FromRoute] Guid petId,
        [FromServices] DeletePetHandler handler,
        CancellationToken cancellationToken = default)
    {
        DeletePetCommand command = new(id, petId);
        CSharpFunctionalExtensions.Result<Guid, ErrorList> result = await handler.Handle(command, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpDelete("{id:guid}/pet/{petId:guid}/hard")]
    public async Task<ActionResult> HardDeletePet([FromRoute] Guid id, [FromRoute] Guid petId,
        [FromServices] HardDeletePetHandler handler,
        CancellationToken cancellationToken = default)
    {
        HardDeletePetCommand command = new(id, petId);
        CSharpFunctionalExtensions.Result<Guid, ErrorList> result = await handler.Handle(command, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpPost("{id:guid}/pet/{petId:guid}/files")]
    public async Task<ActionResult> UploadFilesToPet(
    [FromRoute] Guid id,
    [FromRoute] Guid petId,
    [FromForm] IFormFileCollection files,
    [FromServices] UploadFilesToPetHandler handler,
    CancellationToken cancellationToken)
    {
        await using FormFileProcessor fileProcessor = new();
        List<UploadFileDto> fileDtos = fileProcessor.Process(files);

        UploadFilesToPetCommand command = new(id, petId, fileDtos);

        CSharpFunctionalExtensions.Result<Guid, ErrorList> result = await handler.Handle(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpPut("{id:guid}/pet/{petId:guid}/mainphoto")]
    public async Task<ActionResult> SetPetMainPhoto([FromRoute] Guid id, [FromRoute] Guid petId,
        [FromBody] UpdatePetMainPhotoRequest request,
        [FromServices] UpdatePetMainPhotoHandler handler,
        CancellationToken cancellationToken = default)
    {
        UpdatePetMainPhotoCommand command = request.ToUpdatePetMainPhotoCommand(id, petId);
        CSharpFunctionalExtensions.Result<Guid, ErrorList> result = await handler.Handle(command, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }
}