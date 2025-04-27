namespace PetFoster.Application.DTO.Volunteer
{
    public sealed class PetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid SpecieId { get; set; }

        public string Description { get; set; }

        public Guid BreedId { get; set; }

        public string Coloration { get; set; }

        public string Health { get; set; }        

        public double Weight { get; set; }

        public double Height { get; set; }

        public string PhoneNumber { get; set; }

        public bool Castrated { get; set; }

        public DateTimeOffset? BirthDay { get; set; }

        public bool Vaccinated { get; set; }

        public int Position { get; set; }

        public string AssistanceStatus { get; set; }

        public List<PetFileDto> PetFiles { get; set; }
    }
}
