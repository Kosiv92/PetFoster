using CSharpFunctionalExtensions;
using System.Text.Json.Serialization;

namespace PetFoster.SharedKernel.ValueObjects;

public sealed class Description : ComparableValueObject
{
    public const int MAX_DESCRIPTION_LENGTH = 500;

    [JsonConstructor]
    private Description(string? value)
    {
        Value = value;
    }

    public string? Value { get; }

    public static Result<Description, Error> Create(string? value)
    {
        return value != null && value.Length > MAX_DESCRIPTION_LENGTH
            ? (Result<Description, Error>)Errors.General.ValueIsInvalid(
                $"Description cannot contain more than {MAX_DESCRIPTION_LENGTH} characters")
            : (Result<Description, Error>)new Description(value);
    }

    public static Result<Description, Error> Empty()
    {
        return Create(null);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}
