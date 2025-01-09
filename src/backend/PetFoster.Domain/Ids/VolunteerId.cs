using CSharpFunctionalExtensions;

namespace PetFoster.Domain.Ids
{
    public sealed class VolunteerId : ComparableValueObject
    {
        private VolunteerId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; }

        public static VolunteerId Empty() => new(Guid.Empty);
        public static VolunteerId Create(Guid id) => new(id);

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Value;
        }
    }

}
