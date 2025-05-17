using PetFoster.Core.Abstractions;

namespace PetFoster.Species.Application.SpecieManagement.GetSpecies
{
    public sealed record GetSpeciesWithPaginationQuery(
        int? PositionFrom,
        int? PositionTo,
        int Page,
        int PageSize) : IQuery;
}
