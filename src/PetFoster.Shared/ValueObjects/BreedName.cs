using CSharpFunctionalExtensions;

namespace PetFoster.Core.ValueObjects;

public sealed class BreedName : ComparableValueObject
{
    public const int MIN_NAME_LENGTH = 2;
    public const int MAX_NAME_LENGTH = 200;

    private BreedName() { }

    public BreedName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<BreedName, Error> Create(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            || value.Length > MAX_NAME_LENGTH
            || value.Length < MIN_NAME_LENGTH
            ? (Result<BreedName, Error>)Errors.General.ValueIsInvalid(NotificationFactory
                .GetErrorForNonNullableValueWithRange(nameof(BreedName), MIN_NAME_LENGTH, MAX_NAME_LENGTH))
            : (Result<BreedName, Error>)new BreedName(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}
