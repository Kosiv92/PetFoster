using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using PetFoster.Domain.Shared;
using PetFoster.WebAPI.DTO.Responses;

namespace PetFoster.WebAPI.Extensions
{
    public static class ResponseExtensions
    {
        public static ActionResult ToResponse(this Error error)
        {
            var statusCode = GetStatusCodeForErrorType(error.Type);

            var responseError = new ResponseError(error.Code, error.Message, null);

            var envelope = Envelope.Error(error.ToErrorList());

            return new ObjectResult(envelope) { StatusCode = statusCode };
        }

        public static ActionResult ToResponse(this ErrorList errorList)
        {
            if (!errorList.Any())
            {
                return new ObjectResult(null)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            var distinctErrorTypes = errorList
                .Select(e => e.Type)
                .Distinct()
                .ToList();

            var statusCode = distinctErrorTypes.Count > 1
                ? StatusCodes.Status500InternalServerError :
                GetStatusCodeForErrorType(errorList.First().Type);

            var envelope = Envelope.Error(errorList);

            return new ObjectResult(envelope) { StatusCode = statusCode };
        }

        private static int GetStatusCodeForErrorType(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Failure => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };                
    }
}
