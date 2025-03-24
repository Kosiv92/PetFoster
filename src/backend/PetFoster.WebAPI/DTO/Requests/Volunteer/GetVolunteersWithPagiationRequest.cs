using PetFoster.Application.Volunteers.GetVolunteers;

namespace PetFoster.WebAPI.DTO.Requests.Volunteer
{
    public sealed record GetVolunteersWithPagiationRequest(int? PositionFrom,
        int? PositionTo,
        int Page,
        int PageSize)
    {        
        public GetVoluteersWithPaginationQuery ToQuery() 
            => new(PositionFrom, PositionTo, Page, PageSize);
    }
}