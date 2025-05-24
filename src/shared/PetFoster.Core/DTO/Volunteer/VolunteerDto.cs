namespace PetFoster.Core.DTO.Volunteer
{
    public sealed class VolunteerDto
    {
        public Guid Id { get; init; }
        public required string FirstName { get; init; }

        public required string LastName { get; init; }

        public string? Patronymic { get; init; }

        public required string Email { get; init; }

        public required string Description { get; init; }

        public int WorkExpirience { get; init; }

        public required string PhoneNumber { get; init; }

        public bool IsDeleted { get; set; }

        public required List<AssistanceRequisitesDto> AssistanceRequisitesList
        { get; set; }

        public required List<SocialNetContactsDto> SocialNetContacts
        { get; set; }

    }
}
