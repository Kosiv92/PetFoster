using CSharpFunctionalExtensions;
using PetFoster.Domain.Ids;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Domain.Entities
{
    public sealed class Breed : Entity<BreedId>
    {
        private Breed() { }

        public Breed(BreedId id, BreedName name) : base(id)
        {
            Id = id;
            Name = name;
        }

        public Pet Pet { get; private set; } = null!;

        public BreedId Id { get; private set; }

        public BreedName Name { get; private set; }
    }
}
