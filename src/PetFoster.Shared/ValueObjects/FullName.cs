using CSharpFunctionalExtensions;

namespace PetFoster.Core.ValueObjects;

public sealed class FullName : ComparableValueObject
{
    public const int MIN_NAME_LENGTH = 2;
    public const int MAX_NAME_LENGTH = 100;

    private FullName() { }

    private FullName(string firstName, string lastName, string? patronymic)
    {
        FirstName = firstName;
        LastName = lastName;
        Patronymic = patronymic;
    }

    public string FirstName { get; }

    public string LastName { get; }

    public string? Patronymic { get; }

    public static Result<FullName, Error> Create(
        string firstName, string lastName, string? patronymic)
    {
        return !IsValidNonNullableValue(firstName)
            ? (Result<FullName, Error>)Errors.General.ValueIsInvalid(
                NotificationFactory.GetErrorForNonNullableValueWithRange(
                    nameof(firstName), MIN_NAME_LENGTH, MAX_NAME_LENGTH))
            : !IsValidNonNullableValue(lastName)
            ? (Result<FullName, Error>)Errors.General.ValueIsInvalid(
                NotificationFactory.GetErrorForNonNullableValueWithRange(
                    nameof(lastName), MIN_NAME_LENGTH, MAX_NAME_LENGTH))
            : !IsValidNullableValue(patronymic)
            ? (Result<FullName, Error>)Errors.General.ValueIsInvalid(
                NotificationFactory.GetErrorForNullableValueWithRange(
                    nameof(patronymic), MIN_NAME_LENGTH, MAX_NAME_LENGTH))
            : (Result<FullName, Error>)new FullName(firstName, lastName, patronymic);
    }

    private static bool IsValidNonNullableValue(string property)
    {
        return !string.IsNullOrWhiteSpace(property)
            && property.Length >= MIN_NAME_LENGTH
            && property.Length <= MAX_NAME_LENGTH;
    }

    private static bool IsValidNullableValue(string property)
    {
        return string.IsNullOrWhiteSpace(property)
            || (!string.IsNullOrWhiteSpace(property)
            && property.Length >= MIN_NAME_LENGTH
            && property.Length <= MAX_NAME_LENGTH);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
        yield return Patronymic;
    }

    public override string ToString()
    {
        return string.Join(' ', LastName, FirstName, Patronymic);
    }
};
