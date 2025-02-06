using CSharpFunctionalExtensions;

namespace PetFoster.Domain.Ids
{
    public sealed class BreedId : ComparableValueObject
    {
        private BreedId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; }

        public static BreedId Empty() => new(Guid.Empty);
        public static BreedId Create(Guid id) => new(id);
        public static BreedId NewBreedId() => new(Guid.NewGuid());

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Guid(BreedId breedId)
        {
            ArgumentNullException.ThrowIfNull(breedId);
            return breedId.Value;
        }
    }

}
