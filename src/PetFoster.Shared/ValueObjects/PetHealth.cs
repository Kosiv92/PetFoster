using CSharpFunctionalExtensions;

namespace PetFoster.Core.ValueObjects;

public sealed class PetHealth : ComparableValueObject
{
    public const int MAX_HEALTH_LENGTH = 500;

    private PetHealth() { }

    private PetHealth(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<PetHealth, Error> Create(string value)
    {
        return string.IsNullOrWhiteSpace(value) || value.Length > MAX_HEALTH_LENGTH
            ? (Result<PetHealth, Error>)Errors.General.ValueIsInvalid(
                $"Pet health cannot be empty and contain more than {MAX_HEALTH_LENGTH} characters")
            : (Result<PetHealth, Error>)new PetHealth(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}
