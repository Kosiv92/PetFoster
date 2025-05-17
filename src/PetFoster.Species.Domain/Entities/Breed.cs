using PetFoster.Core.Entities;
using PetFoster.Core.ValueObjects;
using PetFoster.Species.Domain.Ids;

namespace PetFoster.Species.Domain.Entities;

public sealed class Breed : SoftDeletableEntity<BreedId>
{        
    private Breed() { }

    public Breed(BreedId id, BreedName name) : base(id)
    {
        Id = id;
        Name = name;
    }

    public Specie Specie { get; private set; } = null!;                         

    public BreedName Name { get; private set; }
}
