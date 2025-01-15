using Microsoft.AspNetCore.Mvc;
using PetFoster.Domain.Shared;

namespace PetFoster.WebAPI.Extensions
{
    public static class ResponseExtensions
    {
        public static ActionResult ToResponse(this Error error)
        {
            var statusCode = error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Failure => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };

            return new ObjectResult(error) { StatusCode = statusCode };
        }        
    }
}
