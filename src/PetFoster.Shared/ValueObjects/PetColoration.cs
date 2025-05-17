using CSharpFunctionalExtensions;

namespace PetFoster.Core.ValueObjects;

public sealed class PetColoration : ComparableValueObject
{
    public const int MAX_NAME_LENGTH = 100;

    private PetColoration() { }

    public PetColoration(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<PetColoration, Error> Create(string value)
    {
        return string.IsNullOrWhiteSpace(value) || value.Length > MAX_NAME_LENGTH
            ? (Result<PetColoration, Error>)Errors.General.ValueIsInvalid(
                $"Coloration cannot be empty and contain more than {MAX_NAME_LENGTH} characters")
            : (Result<PetColoration, Error>)new PetColoration(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}
