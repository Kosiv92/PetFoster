using CSharpFunctionalExtensions;

namespace PetFoster.SharedKernel.ValueObjects;

public sealed class PhoneNumber : ComparableValueObject
{
    public const string COLUMN_NAME = "phone_number";
    public const int PHONE_NUMBER_LENGTH = 11;

    private PhoneNumber() { }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<PhoneNumber, Error> Create(string value)
    {
        return IsValidValue(value)
            ? new PhoneNumber(value)
            : Errors.General.ValueIsInvalid(
                $"Value must have {PHONE_NUMBER_LENGTH} digits");
    }

    private static bool IsValidValue(string value)
    {
        return !string.IsNullOrWhiteSpace(value)
            && value.Length == PHONE_NUMBER_LENGTH
            && value.All(char.IsDigit);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}
