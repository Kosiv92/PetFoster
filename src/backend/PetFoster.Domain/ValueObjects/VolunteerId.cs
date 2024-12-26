namespace PetFoster.Domain.ValueObjects
{
    public sealed record VolunteerId : IComparable<VolunteerId>
    {
        private VolunteerId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; }

        public static VolunteerId Empty() => new(Guid.Empty);
        public static VolunteerId Create(Guid id) => new(id);
                
        public int CompareTo(VolunteerId? other)
            => this.Value.CompareTo(other?.Value);
    }

}
