using CSharpFunctionalExtensions;

namespace PetFoster.Core.ValueObjects;

public sealed class Address : ComparableValueObject
{
    public const int MAX_REGION_LENGTH = 100;
    public const int MAX_CITY_LENGTH = 100;
    public const int MAX_STREET_LENGTH = 200;
    public const int MAX_HOUSE_LENGTH = 50;
    public const int MAX_APARTMENT_LENGTH = 50;

    private Address() { }

    private Address(string region, string city,
        string street, string houseNumber, string? apartmentNumber)
    {
        Region = region;
        City = city;
        Street = street;
        HouseNumber = houseNumber;
        ApartmentNumber = apartmentNumber;
    }

    public string Region { get; }

    public string City { get; }

    public string Street { get; }

    public string HouseNumber { get; }

    public string? ApartmentNumber { get; }

    public static Result<Address, Error> Create(string region, string city,
        string street, string houseNumber, string? apartmentNumber)
    {
        if (IsValidNonNullableValue(region, MAX_REGION_LENGTH))
        {
            return Errors.General.ValueIsInvalid(
                NotificationFactory.GetErrorForNonNullableValueWithMaxLimit(nameof(region), MAX_REGION_LENGTH));
        }

        return IsValidNonNullableValue(city, MAX_CITY_LENGTH)
            ? (Result<Address, Error>)Errors.General.ValueIsInvalid(
                NotificationFactory.GetErrorForNonNullableValueWithMaxLimit(nameof(city), MAX_CITY_LENGTH))
            : IsValidNonNullableValue(street, MAX_STREET_LENGTH)
            ? (Result<Address, Error>)Errors.General.ValueIsInvalid(
                NotificationFactory.GetErrorForNonNullableValueWithMaxLimit(nameof(street), MAX_STREET_LENGTH))
            : IsValidNonNullableValue(houseNumber, MAX_HOUSE_LENGTH)
            ? (Result<Address, Error>)Errors.General.ValueIsInvalid(
                NotificationFactory.GetErrorForNonNullableValueWithMaxLimit(nameof(houseNumber), MAX_HOUSE_LENGTH))
            : IsValidNullableValue(apartmentNumber, MAX_APARTMENT_LENGTH)
            ? (Result<Address, Error>)Errors.General.ValueIsInvalid(
                NotificationFactory.GetErrorForNullableValueWithMaxLimit(nameof(apartmentNumber), MAX_APARTMENT_LENGTH))
            : (Result<Address, Error>)new Address(region, city, street, houseNumber, apartmentNumber);
    }

    private static bool IsValidNonNullableValue(string property, int maxLimit)
    {
        return string.IsNullOrWhiteSpace(property) || property.Length > maxLimit;
    }

    private static bool IsValidNullableValue(string property, int maxLimit)
    {
        return string.IsNullOrWhiteSpace(property) || (!string.IsNullOrWhiteSpace(property) && property.Length > maxLimit);
    }

    public override string? ToString()
    {
        return string.Join(',', Region, City, Street,
                HouseNumber, ApartmentNumber);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Region;
        yield return City;
        yield return Street;
        yield return HouseNumber;
        yield return ApartmentNumber;
    }
}
