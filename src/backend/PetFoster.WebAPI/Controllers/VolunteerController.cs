using Microsoft.AspNetCore.Mvc;
using PetFoster.Application.Volunteers.AddPet;
using PetFoster.Application.Volunteers.CreateVolunteer;
using PetFoster.Application.Volunteers.DeletePet;
using PetFoster.Application.Volunteers.DeleteVolunteer;
using PetFoster.Application.Volunteers.GetVolunteer;
using PetFoster.Application.Volunteers.GetVolunteers;
using PetFoster.Application.Volunteers.UpdatePersonalInfo;
using PetFoster.Application.Volunteers.UpdatePetStatus;
using PetFoster.Application.Volunteers.UpdateRequisites;
using PetFoster.Application.Volunteers.UpdateSocialNet;
using PetFoster.Application.Volunteers.UploadFilesToPet;
using PetFoster.WebAPI.DTO.Requests.Volunteer;
using PetFoster.WebAPI.Extensions;
using PetFoster.WebAPI.Processors;

namespace PetFoster.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VolunteerController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetVolunteers([FromQuery] GetVolunteersWithPagiationRequest request,
            [FromServices] GetVoluteersWithPaginationHandler handler,
            CancellationToken cancellationToken = default)
        {
            var query = request.ToQuery();

            var volunteerDtos = await handler.Handle(query, cancellationToken);

            return Ok(volunteerDtos);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult> GetVolunteer([FromRoute] Guid id,
            [FromServices] GetVolunteerHandler handler,
            CancellationToken cancellationToken = default)
        {
            var query = new GetVolunteerByIdQuery(id);

            var volunteerDto = await handler.Handle(query, cancellationToken);

            return Ok(volunteerDto);
        }


        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromServices] CreateVolunteerHandler handler,
            [FromBody] CreateVolunteerRequest request, CancellationToken cancellationToken = default)
        {
            var command = request.ToCreateVolunteerCommand();

            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }

        [HttpPut("{id:guid}/personal-info")]
        public async Task<IActionResult> UpdatePersonalInfo([FromRoute] Guid id,
            [FromServices] UpdateVolunteerPersonalInfoHandler handler,
            [FromBody] UpdateVolunteerPersonalInfoRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = request.ToUpdateVolunteerPersonalInfoCommand(id);

            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }

        [HttpPut("{id:guid}/social-net")]
        public async Task<IActionResult> UpdateSocialNetInfo([FromRoute] Guid id,
            [FromServices] UpdateVolunteerSocialNetHandler handler,
            [FromBody] UpdateSocialNetInfoVolunteerRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = request.ToUpdateVolunteerSocialNetCommand(id);

            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }

        [HttpPut("{id:guid}/requisites")]
        public async Task<IActionResult> UpdateRequisites([FromRoute] Guid id,
            [FromServices] UpdateVolunteerRequisitesHandler handler,
            [FromBody] UpdateRequisitesInfoVolunteerRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = request.ToUpdateVolunteerRequisitesCommand(id);

            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id,
            [FromServices] DeleteVolunteerHandler handler, 
            CancellationToken cancellationToken = default)
        {
            var command = new DeleteVolunteerCommand(id);

            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }

        [HttpPost("{id:guid}/pet")]
        public async Task<ActionResult> AddPet([FromRoute] Guid id,
            [FromBody] AddPetRequest request,
            [FromServices] AddPetHandler handler,
            CancellationToken cancellationToken = default)
        {
            var command = request.ToAddPetCommand(id);

            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }

        [HttpPut("{id:guid}/pet/{petId:guid}/status")]
        public async Task<ActionResult> UpdatePetStatus([FromRoute] Guid id, [FromRoute] Guid petId,
            [FromBody] UpdatePetStatusRequest request,
            [FromServices] UpdatePetStatusHandler handler,
            CancellationToken cancellationToken = default)
        {
            var command = request.ToCommand(id, petId);
            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }

        [HttpDelete("{id:guid}/pet/{petId:guid}")]
        public async Task<ActionResult> DeletePet([FromRoute] Guid id, [FromRoute] Guid petId,             
            [FromServices] DeletePetHandler handler,
            CancellationToken cancellationToken = default)
        {
            var command = new DeletePetCommand(id, petId);
            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }

        [HttpDelete("{id:guid}/pet/{petId:guid}/hard")]
        public async Task<ActionResult> HardDeletePet([FromRoute] Guid id, [FromRoute] Guid petId,
            [FromServices] HardDeletePetHandler handler,
            CancellationToken cancellationToken = default)
        {
            var command = new HardDeletePetCommand(id, petId);
            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }

        [HttpPost("{id:guid}/pet/{petId:guid}/files")]
        public async Task<ActionResult> UploadFilesToPet(
        [FromRoute] Guid id,
        [FromRoute] Guid petId,
        [FromForm] IFormFileCollection files,
        [FromServices] UploadFilesToPetHandler handler,
        CancellationToken cancellationToken)
        {
            await using var fileProcessor = new FormFileProcessor();
            var fileDtos = fileProcessor.Process(files);

            var command = new UploadFilesToPetCommand(id, petId, fileDtos);

            var result = await handler.Handle(command, cancellationToken);
            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }
    }
}