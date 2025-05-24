namespace PetFoster.Volunteers.Contracts.DTO.Requests.Volunteers;

public sealed record GetVolunteersWithPagiationRequest(int? PositionFrom,
    int? PositionTo,
    int Page,
    int PageSize);