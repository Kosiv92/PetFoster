using PetFoster.Volunteers.Application.VolunteerManagement.GetVolunteers;

namespace PetFoster.Volunteers.Controllers.DTO.Requests.Volunteers;

public sealed record GetVolunteersWithPagiationRequest(int? PositionFrom,
    int? PositionTo,
    int Page,
    int PageSize)
{
    public GetVoluteersWithPaginationQuery ToQuery()
    {
        return new(PositionFrom, PositionTo, Page, PageSize);
    }
}