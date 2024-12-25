using CSharpFunctionalExtensions;
using PetFoster.Domain.Enums;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Domain.Entities
{
    public sealed class Pet : Entity<PetId>
    {
        public Pet(PetId id, PetName name, Species specie, Description description, Breed breed, 
            PetColoration coloration, PetHealth health, Address address, double weight, double height, 
            PhoneNumber ownerPhoneNumber, bool isCastrated, DateTimeOffset? birthDay, bool isVaccinated, 
            AssistanceStatus assistanceStatus, AssistanceRequisites assistanceRequisites) : base(id)
        {
            Id = id;
            Name = name;
            Specie = specie;
            Description = description;
            Breed = breed;
            Coloration = coloration;
            Health = health;
            Address = address;
            Weight = weight;
            Height = height;
            OwnerPhoneNumber = ownerPhoneNumber;
            IsCastrated = isCastrated;
            BirthDay = birthDay;
            IsVaccinated = isVaccinated;
            AssistanceStatus = assistanceStatus;
            AssistanceRequisites = assistanceRequisites;
            CreatedDate = DateTimeOffset.Now;
        }

        private Pet() { }

        public PetId Id { get; private set; }

        public PetName Name { get; private set; }

        public Species Specie { get; private set; }

        public Description Description { get; private set; }

        public Breed Breed { get; private set; }

        public PetColoration Coloration { get; private set; }

        public PetHealth Health { get; private set; }

        public Address Address { get; private set; }

        public double Weight { get; private set; }

        public double Height { get; private set; }

        public PhoneNumber OwnerPhoneNumber { get; private set; }

        public bool IsCastrated { get; private set; }

        public DateTimeOffset? BirthDay { get; set; }

        public bool IsVaccinated { get; private set; }

        public AssistanceStatus AssistanceStatus { get; private set; }

        public AssistanceRequisites AssistanceRequisites { get; private set; }

        public DateTimeOffset CreatedDate { get; private set; }

    }
}
