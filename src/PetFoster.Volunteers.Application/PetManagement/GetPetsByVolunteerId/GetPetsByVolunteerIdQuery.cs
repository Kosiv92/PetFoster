using PetFoster.Volunteers.Domain.Ids;

namespace PetFoster.Volunteers.Application.PetManagement.GetPetsByVolunteerId;

public sealed record GetPetsByVolunteerIdQuery(VolunteerId VolunteerId);
