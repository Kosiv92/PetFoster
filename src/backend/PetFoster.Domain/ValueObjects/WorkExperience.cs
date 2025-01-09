using CSharpFunctionalExtensions;

namespace PetFoster.Domain.ValueObjects
{
    public sealed class WorkExperience : ComparableValueObject
    {
        public const string COLUMN_NAME = "work_expirience";

        public int Value { get; }

        private WorkExperience(int value) => Value = value;

        public static Result<WorkExperience> Create(int value) 
            => value < 0 
            ? Result.Failure<WorkExperience>("Work experience cannot be negative") 
            : Result.Success<WorkExperience>(new WorkExperience(value));

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Value;
        }
    }
}
