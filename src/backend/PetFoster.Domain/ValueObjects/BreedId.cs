namespace PetFoster.Domain.ValueObjects
{
    public sealed record BreedId : IComparable<BreedId>
    {
        private BreedId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; }

        public static BreedId Empty() => new(Guid.Empty);
        public static BreedId Create(Guid id) => new(id);

        public int CompareTo(BreedId? other)
        => this.Value.CompareTo(other?.Value);
    }

}
