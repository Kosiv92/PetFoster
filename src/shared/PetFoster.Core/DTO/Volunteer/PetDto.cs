namespace PetFoster.Core.DTO.Volunteer
{
    public sealed class PetDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        public Guid SpecieId { get; set; }

        public required string Description { get; set; }

        public Guid BreedId { get; set; }

        public required string Coloration { get; set; }

        public required string Health { get; set; }

        public double Weight { get; set; }

        public double Height { get; set; }

        public required string PhoneNumber { get; set; }

        public bool Castrated { get; set; }

        public DateTimeOffset? BirthDay { get; set; }

        public bool Vaccinated { get; set; }

        public int Position { get; set; }

        public required string AssistanceStatus { get; set; }

        public required List<PetFileDto> PetFiles { get; set; }
    }
}
