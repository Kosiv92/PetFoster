using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO;

namespace PetFoster.Volunteers.Application.PetManagement.GetPets;

public sealed record GetPetsWithPaginationQuery(
    int Page,
    int PageSize,
    string? SortBy,
    bool SortAsc,
    List<FilterItemDto>? FilterList) : IQuery;

