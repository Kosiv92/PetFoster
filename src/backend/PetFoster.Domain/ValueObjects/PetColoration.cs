using CSharpFunctionalExtensions;

namespace PetFoster.Domain.ValueObjects
{
    public sealed class PetColoration : ComparableValueObject
    {
        public const int MAX_NAME_LENGTH = 100;

        private PetColoration() { }

        public PetColoration(string value) => Value = value;

        public string Value { get; }

        public static Result<PetColoration> Create(string value)
        {
            if (String.IsNullOrWhiteSpace(value) || value.Length > PetColoration.MAX_NAME_LENGTH)
                return Result.Failure<PetColoration>(
                    $"Coloration cannot be empty and contain more than {PetColoration.MAX_NAME_LENGTH} characters");

            return Result.Success<PetColoration>(new PetColoration(value));
        }

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Value;
        }
    }   
}
