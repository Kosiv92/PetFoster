using PetFoster.Application.Interfaces;

namespace PetFoster.Application.Volunteers.GetVolunteers
{
    public sealed record GetVoluteersWithPaginationQuery(
        int? PositionFrom, 
        int? PositionTo, 
        int Page, 
        int PageSize) : IQuery;    
}
