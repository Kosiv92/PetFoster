namespace PetFoster.Core.DTO.Volunteer
{
    public sealed record AddressDto(string Region, string City,
        string Street, string HouseNumber, string? ApartmentNumber);

}
