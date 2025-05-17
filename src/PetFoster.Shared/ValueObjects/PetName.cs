using CSharpFunctionalExtensions;

namespace PetFoster.Core.ValueObjects;

public sealed class PetName : ComparableValueObject
{
    public const int MIN_NAME_LENGTH = 1;
    public const int MAX_NAME_LENGTH = 100;

    private PetName() { }

    private PetName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<PetName, Error> Create(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            || value.Length > MAX_NAME_LENGTH
            || value.Length < MIN_NAME_LENGTH
            ? (Result<PetName, Error>)Errors.General.ValueIsInvalid(
                NotificationFactory.GetErrorForNonNullableValueWithRange(
                    "pet name", MIN_NAME_LENGTH, MAX_NAME_LENGTH))
            : (Result<PetName, Error>)new PetName(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}
