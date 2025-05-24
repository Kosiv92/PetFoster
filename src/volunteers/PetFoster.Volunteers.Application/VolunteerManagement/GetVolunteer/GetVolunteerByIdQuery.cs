using PetFoster.Core.Abstractions;

namespace PetFoster.Volunteers.Application.VolunteerManagement.GetVolunteer;

public sealed record GetVolunteerByIdQuery(Guid Id) : IQuery;
