using PetFoster.Application.Volunteers.GetPets;

namespace PetFoster.WebAPI.DTO.Requests.Volunteer
{
    public sealed record GetPetsWithPagiationRequest(
        int Page,
        int PageSize,
        string? SortBy,
        bool SortAsc,
        Dictionary<string, (string, string)>? FilterList)
    {
        public GetPetsWithPaginationQuery ToQuery()        
            => new GetPetsWithPaginationQuery(            
            this.Page,
            this.PageSize,
            this.SortBy,
            this.SortAsc,
            this.FilterList);
    }
    
}
