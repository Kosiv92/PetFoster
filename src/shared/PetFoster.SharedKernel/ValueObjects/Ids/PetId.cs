using CSharpFunctionalExtensions;

namespace PetFoster.SharedKernel.ValueObjects.Ids;

public sealed class PetId : ComparableValueObject
{
    private PetId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static PetId Empty()
    {
        return new(Guid.Empty);
    }

    public static PetId Create(Guid id)
    {
        return new(id);
    }

    public static PetId NewPetId()
    {
        return new(Guid.NewGuid());
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator Guid(PetId petId)
    {
        ArgumentNullException.ThrowIfNull(petId);
        return petId.Value;
    }
}
