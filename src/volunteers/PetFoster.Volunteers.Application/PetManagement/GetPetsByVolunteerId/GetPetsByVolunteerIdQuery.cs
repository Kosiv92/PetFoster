using PetFoster.SharedKernel.ValueObjects.Ids;

namespace PetFoster.Volunteers.Application.PetManagement.GetPetsByVolunteerId;

public sealed record GetPetsByVolunteerIdQuery(VolunteerId VolunteerId);
