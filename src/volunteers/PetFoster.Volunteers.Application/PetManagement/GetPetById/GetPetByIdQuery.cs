using PetFoster.Core.Abstractions;

namespace PetFoster.Volunteers.Application.PetManagement.GetPetById;

public sealed record GetPetByIdQuery(Guid VolunteerId, Guid PetId)
    : IQuery;
