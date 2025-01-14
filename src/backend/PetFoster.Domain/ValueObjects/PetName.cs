using CSharpFunctionalExtensions;
using PetFoster.Domain.Shared;

namespace PetFoster.Domain.ValueObjects
{
    public sealed class PetName : ComparableValueObject
    {
        public const int MIN_NAME_LENGTH = 1;
        public const int MAX_NAME_LENGTH = 100;

        private PetName() { }

        private PetName(string value)
            => Value = value;

        public string Value { get; }

        public static Result<PetName, Error> Create(string value)
        {
            if (String.IsNullOrWhiteSpace(value)
                || value.Length > PetName.MAX_NAME_LENGTH
                || value.Length < MIN_NAME_LENGTH)
                return Errors.General.ValueIsInvalid(
                    NotificationFactory.GetErrorForNonNullableValueWithRange("pet name", MIN_NAME_LENGTH, MAX_NAME_LENGTH));

            return new PetName(value);
        }

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Value;
        }
    }
}
