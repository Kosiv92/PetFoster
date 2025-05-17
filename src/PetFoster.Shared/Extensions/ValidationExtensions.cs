using FluentValidation.Results;
using PetFoster.Core;

namespace PetFoster.Core.Extensions
{
    public static class ValidationExtensions
    {
        public static ErrorList ToErrorList(this ValidationResult validationResult)
        {
            return validationResult.Errors
                    .Select(e =>
                    {
                        Error error = Error.Deserialize(e.ErrorMessage);
                        return Error.Validation(error.Code, error.Message, e.PropertyName);
                    })
                    .ToList();
        }
    }
}
