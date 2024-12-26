using CSharpFunctionalExtensions;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Domain.Entities
{
    public sealed class Volunteer : Entity<VolunteerId>
    {
        public Volunteer(VolunteerId id, FullName fullName, string email, Description description, 
            int workExperienceInYears, PhoneNumber phoneNumber, AssistanceRequisites assistanceRequisites, 
            IReadOnlyList<SocialNetContact> socialNetContacts, IReadOnlyList<Pet> fosteredAnimals) : base(id)
        {
            Id = id;
            FullName = fullName;
            Email = email;
            Description = description;
            WorkExperienceInYears = workExperienceInYears;
            PhoneNumber = phoneNumber;
            AssistanceRequisites = assistanceRequisites;
            SocialNetContacts = socialNetContacts;
            FosteredAnimals = fosteredAnimals;
        }

        private Volunteer() { }

        public VolunteerId Id { get; private set; }

        public FullName FullName { get; private set; }

        public string Email { get; private set; }

        public Description Description { get; private set; }

        public int WorkExperienceInYears { get; private set; }

        public PhoneNumber PhoneNumber { get; private set; }

        public AssistanceRequisites AssistanceRequisites { get; private set; }

        public IReadOnlyList<SocialNetContact> SocialNetContacts { get; private set; }

        public IReadOnlyList<Pet> FosteredAnimals { get; private set; }

        public int FoundHomeAnimals => FosteredAnimals?
            .Count(a => a.AssistanceStatus == Enums.AssistanceStatus.FoundHome) ?? 0;

        public int LookingForHomeAnimals => FosteredAnimals?
            .Count(a => a.AssistanceStatus == Enums.AssistanceStatus.LookingForHome) ?? 0;

        public int NeedsHelpAnimals => FosteredAnimals?
            .Count(a => a.AssistanceStatus == Enums.AssistanceStatus.NeedsHelp) ?? 0;

    }
}
