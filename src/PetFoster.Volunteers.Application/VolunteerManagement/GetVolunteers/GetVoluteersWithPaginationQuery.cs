using PetFoster.Core.Abstractions;

namespace PetFoster.Volunteers.Application.VolunteerManagement.GetVolunteers;

public sealed record GetVoluteersWithPaginationQuery(
    int? PositionFrom,
    int? PositionTo,
    int Page,
    int PageSize) : IQuery;
