using CSharpFunctionalExtensions;
using PetFoster.Domain.Shared;

namespace PetFoster.Domain.ValueObjects
{
    public sealed class PetHealth : ComparableValueObject
    {
        public const int MAX_HEALTH_LENGTH = 500;

        private PetHealth() { }

        private PetHealth(string value) => Value = value;

        public string Value { get; }

        public static Result<PetHealth, Error> Create(string value)
        {
            if (String.IsNullOrWhiteSpace(value) || value.Length > MAX_HEALTH_LENGTH)
                return Errors.General.ValueIsInvalid(
                    $"Pet health cannot be empty and contain more than {PetHealth.MAX_HEALTH_LENGTH} characters");

            return new PetHealth(value);
        }

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Value;
        }
    }
}
