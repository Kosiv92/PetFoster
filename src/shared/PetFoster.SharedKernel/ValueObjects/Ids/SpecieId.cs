using CSharpFunctionalExtensions;

namespace PetFoster.SharedKernel.ValueObjects.Ids;

public sealed class SpecieId : ComparableValueObject
{
    private SpecieId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static SpecieId Empty()
    {
        return new(Guid.Empty);
    }

    public static SpecieId Create(Guid id)
    {
        return new(id);
    }

    public static SpecieId NewSpecieId()
    {
        return new(Guid.NewGuid());
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator Guid(SpecieId specieId)
    {
        ArgumentNullException.ThrowIfNull(specieId);
        return specieId.Value;
    }
}
