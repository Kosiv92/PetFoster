using CSharpFunctionalExtensions;

namespace PetFoster.Core.ValueObjects;

public sealed class Position
    : ComparableValueObject, IEquatable<Position>
{
    private Position(int value)
    {
        Value = value;
    }

    public int Value { get; }
    public static Result<Position, Error> Create(int value)
    {
        return value <= 0
            ? (Result<Position, Error>)Errors.General.ValueIsInvalid(
                "Serial number must be greater than zero")
            : (Result<Position, Error>)new Position(value);
    }

    public Result<Position, Error> Forward()
    {
        return Create(Value + 1);
    }

    public Result<Position, Error> Back()
    {
        return Create(Value - 1);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }

    public bool Equals(Position? other)
    {
        return other != null && other.Value == Value;
    }

    public static Position First => new(1);
}
