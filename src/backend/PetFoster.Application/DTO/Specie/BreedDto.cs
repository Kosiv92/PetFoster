namespace PetFoster.Application.DTO.Specie
{
    public sealed class BreedDto()
    {
        public Guid Id { get; set; }

        public Guid SpecieId { get; set; }

        public string Name { get; set; }
    }    
}
