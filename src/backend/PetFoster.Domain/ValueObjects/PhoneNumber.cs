using CSharpFunctionalExtensions;

namespace PetFoster.Domain.ValueObjects
{
    public sealed record PhoneNumber
    {
        private PhoneNumber() { }

        private PhoneNumber(string value)
            => Value = value;


        public string Value { get; }

        public static Result<PhoneNumber> Create(string value)
         => IsValidValue(value)
            ? Result.Success<PhoneNumber>(new PhoneNumber(value))
            : Result.Failure<PhoneNumber>("Value must have 11 digits");

        private static bool IsValidValue(string value)
        => !String.IsNullOrWhiteSpace(value) 
            && value.Length == 11 
            && value.All(Char.IsDigit);
    }
}
