namespace PetFoster.Domain.Ids
{
    public sealed record SpeciesId : IComparable<SpeciesId>
    {
        private SpeciesId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; }

        public static SpeciesId Empty() => new(Guid.Empty);
        public static SpeciesId Create(Guid id) => new(id);

        public int CompareTo(SpeciesId? other)
        => Value.CompareTo(other?.Value);
    }

}
