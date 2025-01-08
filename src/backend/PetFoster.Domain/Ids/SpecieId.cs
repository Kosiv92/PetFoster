namespace PetFoster.Domain.Ids
{
    public sealed record SpecieId : IComparable<SpecieId>
    {
        private SpecieId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; }

        public static SpecieId Empty() => new(Guid.Empty);
        public static SpecieId Create(Guid id) => new(id);

        public int CompareTo(SpecieId? other)
        => Value.CompareTo(other?.Value);
    }

}
