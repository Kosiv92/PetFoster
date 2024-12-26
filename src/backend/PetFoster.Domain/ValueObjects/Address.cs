namespace PetFoster.Domain.ValueObjects
{
    public sealed record Address
    {
        public string Region { get; }

        public string City { get; }

        public string Street { get; }

        public string HouseNumber { get; }

        public string ApartmentNumber { get; }
    }
}
