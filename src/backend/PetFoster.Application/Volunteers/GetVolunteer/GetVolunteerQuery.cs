using PetFoster.Application.Interfaces;

namespace PetFoster.Application.Volunteers.GetVolunteer
{
    public sealed record GetVolunteerQuery(Guid Id) : IQuery;        
}
