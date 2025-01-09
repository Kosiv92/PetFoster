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

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Value;
        }
    }

}
