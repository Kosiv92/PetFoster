using CSharpFunctionalExtensions;

namespace PetFoster.Domain.Ids
{
    public sealed class SpecieId : ComparableValueObject
    {
        private SpecieId(Guid value)
        {
            Value = value;
        }        

        public Guid Value { get; }

        public static SpecieId Empty() => new(Guid.Empty);
        public static SpecieId Create(Guid id) => new(id);

        public static SpecieId NewSpecieId() => new(Guid.NewGuid());

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Guid(SpecieId specieId)
        {
            ArgumentNullException.ThrowIfNull(specieId);
            return specieId.Value;
        }
    }

}
