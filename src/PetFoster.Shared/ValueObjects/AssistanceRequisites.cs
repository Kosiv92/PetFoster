using CSharpFunctionalExtensions;
using System.Text.Json.Serialization;

namespace PetFoster.Core.ValueObjects;

public sealed class AssistanceRequisites : ComparableValueObject
{
    public const int MAX_NAME_LENGTH = 100;

    [JsonConstructor]
    private AssistanceRequisites(string name, Description description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; }

    public Description Description { get; }

    public static Result<AssistanceRequisites, Error> Create(string name, Description description)
    {
        return string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_LENGTH
            ? (Result<AssistanceRequisites, Error>)Errors.General.ValueIsInvalid(
                $"The name cannot be empty or contain more than {MAX_NAME_LENGTH} characters")
            : (Result<AssistanceRequisites, Error>)new AssistanceRequisites(name, description);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Name;
        yield return Description;
    }
}
