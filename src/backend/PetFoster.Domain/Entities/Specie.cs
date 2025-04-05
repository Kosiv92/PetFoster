using CSharpFunctionalExtensions;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Domain.Entities
{
    public sealed class Specie : SoftDeletableEntity<SpecieId>
    {
        private List<Breed> _breeds = new List<Breed>();

        private Specie() { }

        public Specie(SpecieId id, SpecieName name) : base(id)
        {
            Id = id;
            Name = name;
        }        

        public SpecieName Name { get; private set; }

        public IReadOnlyList<Breed> Breeds => _breeds;

        public UnitResult<Error> AddBreed(Breed breed)
        {
            _breeds.Add(breed);
            return Result.Success<Error>();
        }

        public override void Delete()
        {
            base.Delete();
            foreach (var breed in _breeds)
            {
                breed.Delete();
            }
        }

        public override void Restore()
        {
            base.Restore();
            foreach (var breed in _breeds)
            {
                breed.Restore();
            }
        }
    }
}
