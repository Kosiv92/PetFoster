using PetFoster.Application.Interfaces;

namespace PetFoster.Application.Volunteers.GetPets
{
    public sealed record GetPetsWithPaginationQuery(
        int Page,
        int PageSize,
        string? SortBy,
        bool SortAsc,
        Dictionary<string, (string, string)>? FilterList) : IQuery;
}
