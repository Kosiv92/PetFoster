using CSharpFunctionalExtensions;

namespace PetFoster.Domain
{
    public sealed record Description
    {
        public const int MAX_DESCRIPTION_LENGTH = 500;

        private Description(string? value) => Value = value;

        public string? Value { get; }

        public static Result<Description> Create(string? value)
        {
            if(value != null && value.Length > MAX_DESCRIPTION_LENGTH) 
                return Result.Failure<Description>(value);
        
            return Result.Success<Description>(new Description(value)) ; 
        }

        public static Result<Description> Empty() => Description.Create(null);

    }
    
}
