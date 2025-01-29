using Microsoft.AspNetCore.Mvc;
using PetFoster.Application.Volunteers.CreateVolunteer;
using PetFoster.Application.Volunteers.UpdatePersonalInfo;
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
    }
}