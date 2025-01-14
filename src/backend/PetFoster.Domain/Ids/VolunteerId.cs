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

        public static VolunteerId NewVolunteerId() => new(Guid.NewGuid());

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Guid(VolunteerId volunteerId)
        {
            ArgumentNullException.ThrowIfNull(volunteerId);
            return volunteerId.Value;
        }
    }

}
