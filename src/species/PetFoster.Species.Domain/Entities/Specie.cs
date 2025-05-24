using CSharpFunctionalExtensions;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.Entities;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.SharedKernel.ValueObjects.Ids;

namespace PetFoster.Species.Domain.Entities;

public sealed class Specie : SoftDeletableEntity<SpecieId>
{
    private readonly List<Breed> _breeds = [];

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
        foreach (Breed breed in _breeds)
        {
            breed.Delete();
        }
    }

    public override void Restore()
    {
        base.Restore();
        foreach (Breed breed in _breeds)
        {
            breed.Restore();
        }
    }
}
