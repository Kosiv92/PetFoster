using FluentValidation.Results;
using PetFoster.Domain.Shared;

namespace PetFoster.Application.Extensions
{
    public static class ValidationExtensions
    {
        public static ErrorList ToErrorList(this ValidationResult validationResult)
        {
            return validationResult.Errors
                    .Select(e =>
                    {
                        var error = Error.Deserialize(e.ErrorMessage);
                        return Error.Validation(error.Code, error.Message, e.PropertyName);
                    })
                    .ToList();
        }
    }
}
