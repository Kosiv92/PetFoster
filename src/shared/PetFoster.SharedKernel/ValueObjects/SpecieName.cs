using CSharpFunctionalExtensions;

namespace PetFoster.SharedKernel.ValueObjects;

public sealed class SpecieName : ComparableValueObject
{
    public const int MIN_NAME_LENGTH = 2;
    public const int MAX_NAME_LENGTH = 200;

    private SpecieName() { }

    public SpecieName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<SpecieName, Error> Create(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            || value.Length > MAX_NAME_LENGTH
            || value.Length < MIN_NAME_LENGTH
            ? (Result<SpecieName, Error>)Errors.General.ValueIsInvalid(NotificationFactory
                .GetErrorForNonNullableValueWithRange(nameof(SpecieName),
                MIN_NAME_LENGTH, MAX_NAME_LENGTH))
            : (Result<SpecieName, Error>)new SpecieName(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}
