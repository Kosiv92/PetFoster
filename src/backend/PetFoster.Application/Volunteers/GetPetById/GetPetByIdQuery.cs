using PetFoster.Application.Interfaces;

namespace PetFoster.Application.Volunteers.GetPetByID
{
    public sealed record GetPetByIdQuery(Guid VolunteerId, Guid PetId) 
        : IQuery;
}
