using CSharpFunctionalExtensions;
using PetFoster.Domain.Shared;
using System.Text.Json.Serialization;

namespace PetFoster.Domain.ValueObjects
{
    public sealed class Description : ComparableValueObject
    {
        public const int MAX_DESCRIPTION_LENGTH = 500;

        [JsonConstructorAttribute]
        private Description(string? value) => Value = value;

        public string? Value { get; }

        public static Result<Description, Error> Create(string? value)
        {
            if (value != null && value.Length > MAX_DESCRIPTION_LENGTH)
                return Errors.General.ValueIsInvalid(
                    $"Description cannot contain more than {MAX_DESCRIPTION_LENGTH} characters");

            return new Description(value);
        }

        public static Result<Description, Error> Empty() => Create(null);

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Value;
        }
    }

}
