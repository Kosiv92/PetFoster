using PetFoster.Core.DTO;
using PetFoster.Volunteers.Application.PetManagement.GetPets;

namespace PetFoster.Volunteers.Controllers.DTO.Requests.Pets;

public sealed record GetPetsWithPagiationRequest(
    int Page,
    int PageSize,
    string? SortBy,
    bool SortAsc,
    List<FilterItemDto>? FilterList)
{
    public GetPetsWithPaginationQuery ToQuery()
    {
        return new GetPetsWithPaginationQuery(
            Page,
            PageSize,
            SortBy,
            SortAsc,
            FilterList);
    }
}

