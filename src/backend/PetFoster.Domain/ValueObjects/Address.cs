using CSharpFunctionalExtensions;
using PetFoster.Domain.Shared;

namespace PetFoster.Domain.ValueObjects
{
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
            if (IsValidNonNullableValue(region, Address.MAX_REGION_LENGTH))
                return Errors.General.ValueIsInvalid(
                    NotificationFactory.GetErrorForNonNullableValueWithMaxLimit(nameof(region), Address.MAX_REGION_LENGTH));

            if (IsValidNonNullableValue(city, Address.MAX_CITY_LENGTH))
                return Errors.General.ValueIsInvalid(
                    NotificationFactory.GetErrorForNonNullableValueWithMaxLimit(nameof(city), Address.MAX_CITY_LENGTH));

            if (IsValidNonNullableValue(street, Address.MAX_STREET_LENGTH))
                return Errors.General.ValueIsInvalid(
                    NotificationFactory.GetErrorForNonNullableValueWithMaxLimit(nameof(street), Address.MAX_STREET_LENGTH));

            if (IsValidNonNullableValue(houseNumber, Address.MAX_HOUSE_LENGTH))
                return Errors.General.ValueIsInvalid(
                    NotificationFactory.GetErrorForNonNullableValueWithMaxLimit(nameof(houseNumber), Address.MAX_HOUSE_LENGTH));

            if (IsValidNullableValue(apartmentNumber, Address.MAX_APARTMENT_LENGTH))
                return Errors.General.ValueIsInvalid(
                    NotificationFactory.GetErrorForNullableValueWithMaxLimit(nameof(apartmentNumber), Address.MAX_APARTMENT_LENGTH));

            return new Address(region, city, street, houseNumber, apartmentNumber);
        }

        private static bool IsValidNonNullableValue(string property, int maxLimit)
            => String.IsNullOrWhiteSpace(property) || property.Length > maxLimit;

        private static bool IsValidNullableValue(string property, int maxLimit)
            => String.IsNullOrWhiteSpace(property) || (!String.IsNullOrWhiteSpace(property) && property.Length > maxLimit);

        public override string? ToString()
            => String.Join(',' , this.Region, this.City, this.Street, 
                this.HouseNumber, this.ApartmentNumber);

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Region;
            yield return City;
            yield return Street;
            yield return HouseNumber;
            yield return ApartmentNumber;
        }
    }
}
