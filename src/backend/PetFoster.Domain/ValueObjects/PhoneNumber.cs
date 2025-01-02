using CSharpFunctionalExtensions;

namespace PetFoster.Domain.ValueObjects
{
    public sealed record PhoneNumber
    {
        public const string COLUMN_NAME = "phone_number";
        public const int PHONE_NUMBER_LENGTH = 11;        

        private PhoneNumber() { }

        private PhoneNumber(string value)
            => Value = value;

        public string Value { get; }

        public static Result<PhoneNumber> Create(string value)
         => IsValidValue(value)
            ? Result.Success<PhoneNumber>(new PhoneNumber(value))
            : Result.Failure<PhoneNumber>($"Value must have {PhoneNumber.PHONE_NUMBER_LENGTH} digits");

        private static bool IsValidValue(string value)
        => !String.IsNullOrWhiteSpace(value) 
            && value.Length == PhoneNumber.PHONE_NUMBER_LENGTH
            && value.All(Char.IsDigit);
    }
}
