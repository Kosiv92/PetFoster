using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.DTO
{
    public sealed class VolunteerDto
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }

        public string LastName { get; init; }

        public string? Patronymic { get; init; }

        public string Email { get; init; }

        public string Description { get; init; }

        public int WorkExpirience { get; init; }

        public string PhoneNumber { get; init; }

        public bool IsDeleted { get; set; }

        public List<AssistanceRequisitesDto> AssistanceRequisitesList
        { get; set; }

        public List<SocialNetContactsDto> SocialNetContacts
        { get; set; }

    }
}
