namespace PetFoster.Application.DTO.Specie
{
    public sealed class SpecieDto()
    {
        public Guid Id { get; set; }

        public string Name{ get; set; }

        public List<BreedDto> Breeds { get; set; }
    }   
}
