using CSharpFunctionalExtensions;

namespace PetFoster.Domain.ValueObjects
{
    public sealed class Characteristics : ComparableValueObject
    {
        public const string WEIGHT_COLUMN_NAME = "weight";

        public const string HEIGHT_COLUMN_NAME = "height";

        private Characteristics() { }

        private Characteristics(double weight, double height) { }

        /// <summary>
        /// Weight in kilograms
        /// </summary>
        public double Weight { get; }

        /// <summary>
        /// Height in centimeters
        /// </summary>
        public double Height { get; }

        public static Result<Characteristics> Create(double weight, double height)
        {
            if (weight <= 0) return Result.Failure<Characteristics>("The weight must be greater than 0");

            if (height <= 0) return Result.Failure<Characteristics>("The height must be greater than 0");

            return Result.Success<Characteristics>(new Characteristics(weight, height));
        }

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return (Weight, Height);
        }
    }
}
