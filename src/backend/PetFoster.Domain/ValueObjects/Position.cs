using CSharpFunctionalExtensions;
using PetFoster.Domain.Shared;

namespace PetFoster.Domain.ValueObjects
{
    public sealed class Position : ComparableValueObject, IEquatable<Position>
    {
        private Position(int value) 
        {
            Value = value;
        }

        public int Value { get; }
        public static Result<Position, Error> Create(int value)
        {
            if(value <= 0)
            {
                return Errors.General.ValueIsInvalid("Serial number must be greater than zero");
            }

            return new Position(value);
        }

        public Result<Position, Error> Forward() => Create(Value + 1);

        public Result<Position, Error> Back() => Create(Value - 1);

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Value;
        }

        public bool Equals(Position? other)
        {
            if (other == null) return false;

            return (other.Value == this.Value);
        }

        public static Position First => new Position(1);        
    }
}
