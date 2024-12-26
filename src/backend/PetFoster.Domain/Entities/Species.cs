using CSharpFunctionalExtensions;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Domain.Entities
{
    public sealed class Species : Entity<SpeciesId>
    {
        private Species() { }

        public Species(SpeciesId id, SpeciesName name, IReadOnlyList<Breed> breeds) : base(id)
        {
            Id = id;
            Name = name;
            Breeds = breeds;
        }

        public SpeciesId Id { get; private set; }

        public SpeciesName Name { get; private set; }

        public IReadOnlyList<Breed> Breeds { get; private set; }
    }
}
