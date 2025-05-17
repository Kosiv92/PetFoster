using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using PetFoster.Core;
using PetFoster.WebAPI.DTO.Responses;

namespace PetFoster.WebAPI.Extensions;

public static class ResponseExtensions
{
    public static ActionResult ToResponse(this Error error)
    {
        int statusCode = GetStatusCodeForErrorType(error.Type);

        ResponseError responseError = new(error.Code, error.Message, null);

        Envelope envelope = Envelope.Error(error.ToErrorList());

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

        List<ErrorType> distinctErrorTypes = errorList
            .Select(e => e.Type)
            .Distinct()
            .ToList();

        int statusCode = distinctErrorTypes.Count > 1
            ? StatusCodes.Status500InternalServerError :
            GetStatusCodeForErrorType(errorList.First().Type);

        Envelope envelope = Envelope.Error(errorList);

        return new ObjectResult(envelope) { StatusCode = statusCode };
    }

    private static int GetStatusCodeForErrorType(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
