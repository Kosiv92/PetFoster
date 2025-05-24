using CSharpFunctionalExtensions;

namespace PetFoster.SharedKernel.ValueObjects.Ids;

public sealed class BreedId : ComparableValueObject
{
    private BreedId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static BreedId Empty()
    {
        return new(Guid.Empty);
    }

    public static BreedId Create(Guid id)
    {
        return new(id);
    }

    public static BreedId NewBreedId()
    {
        return new(Guid.NewGuid());
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator Guid(BreedId breedId)
    {
        ArgumentNullException.ThrowIfNull(breedId);
        return breedId.Value;
    }
}
