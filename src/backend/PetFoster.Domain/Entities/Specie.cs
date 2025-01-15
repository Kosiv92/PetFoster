using CSharpFunctionalExtensions;
using PetFoster.Domain.Ids;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Domain.Entities
{
    public sealed class Specie : Entity<SpecieId>
    {
        private List<Breed> _breeds;

        private Specie() { }

        public Specie(SpecieId id, SpecieName name, List<Breed> breeds) : base(id)
        {
            Id = id;
            Name = name;
            _breeds = breeds;
        }        

        public SpecieName Name { get; private set; }

        public IReadOnlyList<Breed> Breeds => _breeds;
    }
}
