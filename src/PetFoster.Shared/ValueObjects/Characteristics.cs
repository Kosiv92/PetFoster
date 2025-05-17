using CSharpFunctionalExtensions;

namespace PetFoster.Core.ValueObjects;

public sealed class Characteristics : ComparableValueObject
{
    public const string WEIGHT_COLUMN_NAME = "weight";

    public const string HEIGHT_COLUMN_NAME = "height";

    private Characteristics() { }

    private Characteristics(double weight, double height)
    {
        (Weight, Height) = (weight, height);
    }

    /// <summary>
    /// Weight in kilograms
    /// </summary>
    public double Weight { get; }

    /// <summary>
    /// Height in centimeters
    /// </summary>
    public double Height { get; }

    public static Result<Characteristics, Error> Create(double weight, double height)
    {
        return weight <= 0
            ? (Result<Characteristics, Error>)Errors.General.ValueIsInvalid("The weight must be greater than 0")
            : height <= 0 ? (Result<Characteristics, Error>)Errors.General.ValueIsInvalid("The height must be greater than 0") : (Result<Characteristics, Error>)new Characteristics(weight, height);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return (Weight, Height);
    }
}
