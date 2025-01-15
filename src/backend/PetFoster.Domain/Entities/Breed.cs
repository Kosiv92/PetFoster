using CSharpFunctionalExtensions;
using PetFoster.Domain.Ids;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Domain.Entities
{
    public sealed class Breed : Entity<BreedId>
    {
        private List<Pet> _pets;

        private Breed() { }

        public Breed(BreedId id, BreedName name) : base(id)
        {
            Id = id;
            Name = name;
        }

        public Specie Specie { get; private set; } = null!;  
        
        public IReadOnlyCollection<Pet> Pets => _pets;                

        public BreedName Name { get; private set; }
    }
}
