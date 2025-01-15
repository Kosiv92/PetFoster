using CSharpFunctionalExtensions;
using PetFoster.Domain.Shared;

namespace PetFoster.Domain.ValueObjects
{
    public sealed class PhoneNumber : ComparableValueObject
    {
        public const string COLUMN_NAME = "phone_number";
        public const int PHONE_NUMBER_LENGTH = 11;        

        private PhoneNumber() { }

        private PhoneNumber(string value)
            => Value = value;

        public string Value { get; }

        public static Result<PhoneNumber, Error> Create(string value)
         => IsValidValue(value)
            ? new PhoneNumber(value)
            : Errors.General.ValueIsInvalid($"Value must have {PhoneNumber.PHONE_NUMBER_LENGTH} digits");

        private static bool IsValidValue(string value)
        => !String.IsNullOrWhiteSpace(value) 
            && value.Length == PhoneNumber.PHONE_NUMBER_LENGTH
            && value.All(Char.IsDigit);

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Value;
        }
    }
}
