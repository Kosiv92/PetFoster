using PetFoster.Application.Species.GetSpecies;

namespace PetFoster.WebAPI.DTO.Requests.Volunteer
{
    public sealed record GetSpeciesWithPagiationRequest(int? PositionFrom,
        int? PositionTo,
        int Page,
        int PageSize)
    {        
        public GetSpeciesWithPaginationQuery ToQuery() 
            => new(PositionFrom, PositionTo, Page, PageSize);
    }
}