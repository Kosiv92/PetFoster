using CSharpFunctionalExtensions;

namespace PetFoster.SharedKernel.ValueObjects;

public sealed class WorkExperience : ComparableValueObject
{
    public const string COLUMN_NAME = "work_expirience";

    public int Value { get; }

    private WorkExperience(int value)
    {
        Value = value;
    }

    public static Result<WorkExperience, Error> Create(int value)
    {
        return value < 0
            ? Errors.General.ValueIsInvalid("Work experience cannot be negative")
            : new WorkExperience(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}
