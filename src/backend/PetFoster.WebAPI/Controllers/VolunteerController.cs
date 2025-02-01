using Microsoft.AspNetCore.Mvc;
using PetFoster.Application.Volunteers.CreateVolunteer;
using PetFoster.Application.Volunteers.UpdatePersonalInfo;
using PetFoster.Application.Volunteers.UpdateRequisites;
using PetFoster.Application.Volunteers.UpdateSocialNet;
using PetFoster.WebAPI.DTO.Requests.Volunteer;
using PetFoster.WebAPI.Extensions;

namespace PetFoster.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VolunteerController : ControllerBase
    {
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
            [FromServices] UpdatePersonalInfoHandler handler,
            [FromBody] UpdateVolunteerPersonalInfoRequest request,
            CancellationToken cancellationToken = default )
        {
            var command = request.ToUpdateVolunteerPersonalInfoCommand(id);

            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }

        [HttpPut("{id:guid}/social-net")]
        public async Task<IActionResult> UpdateSocialNetInfo([FromRoute] Guid id,
            [FromServices] UpdateSocialNetHandler handler,
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
            [FromServices] UpdateRequisitesHandler handler,
            [FromBody] UpdateRequisitesInfoVolunteerRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = request.ToUpdateVolunteerRequisitesCommand(id);

            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToResponse();

            return Ok(result.Value);
        }
    }
}