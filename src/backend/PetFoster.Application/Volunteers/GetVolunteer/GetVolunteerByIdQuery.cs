using PetFoster.Application.Interfaces;

namespace PetFoster.Application.Volunteers.GetVolunteer
{
    public sealed record GetVolunteerByIdQuery(Guid Id) : IQuery;        
}
