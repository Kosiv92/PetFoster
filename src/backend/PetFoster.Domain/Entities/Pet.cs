using PetFoster.Domain.Enums;
using PetFoster.Domain.Ids;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Domain.Entities
{
    public sealed class Pet : SoftDeletableEntity<PetId>
    {
        public Pet(PetId id, Volunteer volunteer, PetName name, Specie specie, Description description, Breed breed, 
            PetColoration coloration, PetHealth health, Address address, Characteristics characteristics, 
            PhoneNumber ownerPhoneNumber, bool isCastrated, DateTimeOffset? birthDay, bool isVaccinated, 
            AssistanceStatus assistanceStatus, IReadOnlyList<AssistanceRequisites> assistanceRequisitesList) : base(id)
        {
            Id = id;
            Volunteer = volunteer;
            Name = name;
            Specie = specie;
            Description = description;
            Breed = breed;
            Coloration = coloration;
            Health = health;
            Address = address;
            Characteristics = characteristics;
            OwnerPhoneNumber = ownerPhoneNumber;
            IsCastrated = isCastrated;
            BirthDay = birthDay;
            IsVaccinated = isVaccinated;
            AssistanceStatus = assistanceStatus;
            AssistanceRequisitesList = assistanceRequisitesList;
            CreatedDate = DateTimeOffset.Now;
        }

        private Pet() { }        

        public Volunteer Volunteer { get; private set; }

        public PetName Name { get; private set; }

        public Specie Specie { get; private set; }

        public Description Description { get; private set; }

        public Breed Breed { get; private set; }

        public PetColoration Coloration { get; private set; }

        public PetHealth Health { get; private set; }

        public Address Address { get; private set; }
         
        public Characteristics Characteristics { get; private set; }

        public PhoneNumber OwnerPhoneNumber { get; private set; }

        public bool IsCastrated { get; private set; }

        public DateTimeOffset? BirthDay { get; set; }

        public bool IsVaccinated { get; private set; }

        public AssistanceStatus AssistanceStatus { get; private set; }

        public IReadOnlyList<AssistanceRequisites> AssistanceRequisitesList { get; private set; }

        public DateTimeOffset CreatedDate { get; private set; }

    }
}
