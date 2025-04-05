using PetFoster.Application.Interfaces;

namespace PetFoster.Application.Species.GetSpecies
{
    public sealed record GetSpeciesWithPaginationQuery(
        int? PositionFrom,
        int? PositionTo,
        int Page,
        int PageSize) : IQuery;
}
