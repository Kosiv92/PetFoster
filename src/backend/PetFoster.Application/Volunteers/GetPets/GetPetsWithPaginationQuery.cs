using PetFoster.Application.DTO;
using PetFoster.Application.Interfaces;

namespace PetFoster.Application.Volunteers.GetPets;

public sealed record GetPetsWithPaginationQuery(
    int Page,
    int PageSize,
    string? SortBy,
    bool SortAsc,
    List<FilterItemDto>? FilterList) : IQuery;

