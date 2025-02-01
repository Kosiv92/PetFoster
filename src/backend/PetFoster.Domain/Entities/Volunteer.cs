using CSharpFunctionalExtensions;
using PetFoster.Domain.Ids;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Domain.Entities
{
    public sealed class Volunteer : Entity<VolunteerId>
    {
        public Volunteer(VolunteerId id, FullName fullName, 
            Email email, Description description,
            WorkExperience workExperienceInYears, 
            PhoneNumber phoneNumber,
            IEnumerable<AssistanceRequisites> assistanceRequisitesList,
            IEnumerable<SocialNetContact> socialNetContacts, 
            IReadOnlyList<Pet> fosteredAnimals) : base(id)
        {
            Id = id;
            FullName = fullName;
            Email = email;
            Description = description;
            WorkExperienceInYears = workExperienceInYears;
            PhoneNumber = phoneNumber;
            _assistanceRequisites = assistanceRequisitesList.ToList();
            _socialNetContacts = socialNetContacts.ToList();
            FosteredAnimals = fosteredAnimals;
        }

        private Volunteer() { }

        private List<AssistanceRequisites> _assistanceRequisites = new List<AssistanceRequisites>();

        private List<SocialNetContact> _socialNetContacts = new List<SocialNetContact>();

        public VolunteerId Id { get; private set; }

        public FullName FullName { get; private set; }

        public Email Email { get; private set; }

        public Description Description { get; private set; }

        public WorkExperience WorkExperienceInYears { get; private set; }

        public PhoneNumber PhoneNumber { get; private set; }

        public IReadOnlyList<AssistanceRequisites> AssistanceRequisitesList 
            => _assistanceRequisites.AsReadOnly();

        public IReadOnlyList<SocialNetContact> SocialNetContacts 
            => _socialNetContacts.AsReadOnly();

        public IReadOnlyList<Pet> FosteredAnimals { get; private set; }

        public int FoundHomeAnimals => FosteredAnimals?
            .Count(a => a.AssistanceStatus == Enums.AssistanceStatus.FoundHome) ?? 0;

        public int LookingForHomeAnimals => FosteredAnimals?
            .Count(a => a.AssistanceStatus == Enums.AssistanceStatus.LookingForHome) ?? 0;

        public int NeedsHelpAnimals => FosteredAnimals?
            .Count(a => a.AssistanceStatus == Enums.AssistanceStatus.NeedsHelp) ?? 0;

        public void UpdatePersonalInfo(FullName fullName, 
            Email email, 
            PhoneNumber phoneNumber, 
            Description description, 
            WorkExperience workExperience)
        {
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            Description = description;
            WorkExperienceInYears = workExperience;
        }

        public void UpdateSocialNetContacts(IEnumerable<SocialNetContact> socialNetContacts)
            => _socialNetContacts = socialNetContacts.ToList();       

    }
}
