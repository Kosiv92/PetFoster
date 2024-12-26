namespace PetFoster.Domain.ValueObjects
{
    public sealed record PetId : IComparable<PetId>
    {
        private PetId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; }

        public static PetId Empty() => new(Guid.Empty);
        public static PetId Create(Guid id) => new(id);

        public int CompareTo(PetId? other)
        => this.Value.CompareTo(other?.Value);
    }

}
