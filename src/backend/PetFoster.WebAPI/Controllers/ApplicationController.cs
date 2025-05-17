using Microsoft.AspNetCore.Mvc;
using PetFoster.WebAPI.DTO.Responses;

namespace PetFoster.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ApplicationController : ControllerBase
    {
        public override OkObjectResult Ok(object? value)
        {
            Envelope envelope = Envelope.Ok(value);
            return new OkObjectResult(envelope);
        }
    }
}
