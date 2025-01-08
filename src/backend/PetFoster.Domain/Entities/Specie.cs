using CSharpFunctionalExtensions;
using PetFoster.Domain.Ids;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Domain.Entities
{
    public sealed class Specie : Entity<SpecieId>
    {
        private Specie() { }

        public Specie(SpecieId id, SpecieName name, IReadOnlyList<Breed> breeds) : base(id)
        {
            Id = id;
            Name = name;
            Breeds = breeds;
        }

        public IReadOnlyCollection<Pet> Pets { get; private set; } = null!;

        public SpecieId Id { get; private set; }

        public SpecieName Name { get; private set; }

        public IReadOnlyList<Breed> Breeds { get; private set; }
    }
}
