using PetFoster.Core.DTO;

namespace PetFoster.Volunteers.Contracts.DTO.Requests.Pets;

public sealed record GetPetsWithPagiationRequest(
    int Page,
    int PageSize,
    string? SortBy,
    bool SortAsc,
    List<FilterItemDto>? FilterList);
