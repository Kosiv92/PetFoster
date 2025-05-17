using PetFoster.Application.Species.GetSpecies;

namespace PetFoster.WebAPI.DTO.Requests.Specie
{
    public sealed record GetSpeciesWithPagiationRequest(int? PositionFrom,
        int? PositionTo,
        int Page,
        int PageSize)
    {
        public GetSpeciesWithPaginationQuery ToQuery()
        {
            return new(PositionFrom, PositionTo, Page, PageSize);
        }
    }
}