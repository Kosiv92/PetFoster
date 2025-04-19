using CSharpFunctionalExtensions;
using PetFoster.Domain.Enums;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Domain.Entities
{
    public sealed class Volunteer : SoftDeletableEntity<VolunteerId>
    {
        public Volunteer(VolunteerId id, FullName fullName,
            Email email, Description description,
            WorkExperience workExperienceInYears,
            PhoneNumber phoneNumber,
            IEnumerable<AssistanceRequisites> assistanceRequisitesList,
            IEnumerable<SocialNetContact> socialNetContacts) : base(id)
        {
            Id = id;
            FullName = fullName;
            Email = email;
            Description = description;
            WorkExperienceInYears = workExperienceInYears;
            PhoneNumber = phoneNumber;
            _assistanceRequisites = assistanceRequisitesList.ToList();
            _socialNetContacts = socialNetContacts.ToList();            
        }

        private Volunteer() { }

        private List<AssistanceRequisites> _assistanceRequisites = new List<AssistanceRequisites>();

        private List<SocialNetContact> _socialNetContacts = new List<SocialNetContact>();

        private List<Pet> _fosteredPets = new List<Pet>();

        public FullName FullName { get; private set; }

        public Email Email { get; private set; }

        public Description Description { get; private set; }

        public WorkExperience WorkExperienceInYears { get; private set; }

        public PhoneNumber PhoneNumber { get; private set; }

        public IReadOnlyList<AssistanceRequisites> AssistanceRequisitesList
            => _assistanceRequisites.AsReadOnly();

        public IReadOnlyList<SocialNetContact> SocialNetContacts
            => _socialNetContacts.AsReadOnly();

        public IReadOnlyList<Pet> FosteredAnimals
            => _fosteredPets.AsReadOnly();

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

        public void UpdateRequisites(IEnumerable<AssistanceRequisites> requisites)
            => _assistanceRequisites = requisites.ToList();

        public UnitResult<Error> AddPet(Pet pet)
        {
            var serialNumberResult = Position.Create(_fosteredPets.Count + 1);
            if (serialNumberResult.IsFailure) return serialNumberResult.Error;
            pet.SetPositionNumber(serialNumberResult.Value);
            pet.SetVolunteer(this);

            _fosteredPets.Add(pet);
            return Result.Success<Error>();
        }

        public UnitResult<Error> UpdatePetInfo(PetId petId, SpecieId specieId, BreedId breedId, PetName name, 
            Description description, PetColoration coloration, PetHealth health, Address address, 
            Characteristics characteristics, PhoneNumber phone, DateTimeOffset? birthDay, bool isCastrated, 
            bool isVaccinated, AssistanceStatus assistanceStatus,
            List<AssistanceRequisites> assistanceRequisites)
        {
            var pet = _fosteredPets.FirstOrDefault(a => a.Id == petId);
            if (pet == null)
                return Errors.General.ValueIsInvalid(
                    $"Pet with id {petId.Value} not found in volunteer with id {this.Id.Value}");

            return pet.UpdateInfo(specieId, breedId, name, description, coloration, health, address,
            characteristics, phone, birthDay, isCastrated, isVaccinated, assistanceStatus, assistanceRequisites);
        }

        public UnitResult<Error> UpdatePetAssistanceStatus(PetId petId, 
            AssistanceStatus assistanceStatus)
        {
            var pet = _fosteredPets.FirstOrDefault(a => a.Id == petId);
            if (pet == null)
                return Errors.General.ValueIsInvalid(
                    $"Pet with id {petId.Value} not found in volunteer with id {this.Id.Value}");

            return pet.UpdateAssistanceStatus(assistanceStatus);
        }

        public UnitResult<Error> UpdatePetMainPhoto(PetId petId,
            string filePath)
        {
            var pet = _fosteredPets.FirstOrDefault(a => a.Id == petId);
            if (pet == null)
                return Errors.General.ValueIsInvalid(
                    $"Pet with id {petId.Value} not found in volunteer with id {this.Id.Value}");

            return pet.SetMainPhoto(filePath);
        }

        public UnitResult<Error> MovePet(Pet pet, Position newPosition)
        {
            if (pet.Volunteer != this) 
                return Errors.General.ValueIsInvalid("The pet does not belong to this volunteer");

            var currentPosition = pet.Position;

            if (currentPosition == newPosition || _fosteredPets.Count == 1)
                return Result.Success<Error>();

            var adjustedPosition = AdjustNewPositionOutOfRange(newPosition);
            if (adjustedPosition.IsFailure) return adjustedPosition.Error;

            newPosition = adjustedPosition.Value;

            var movePetsResult = MovePetsBetweenPositions(newPosition, currentPosition);
            if (movePetsResult.IsFailure) return movePetsResult.Error;

            pet.Move(newPosition);

            var uniqCounts = _fosteredPets.Select(p => p.Position.Value).Distinct().Count();
            if (uniqCounts != _fosteredPets.Count)
                return Error.Failure("nonuniq.pet.position", "Non-unique animal positions at the volunteer");

            return Result.Success<Error>();
        }

        public UnitResult<Error> MovePetToStart(Pet pet)
            => MovePet(pet, Position.First);

        public UnitResult<Error> MovePetToEnd(Pet pet)
        {
            var lastPosition = Position.Create(_fosteredPets.Count).Value;
            return MovePet(pet, lastPosition);
        }         

        private UnitResult<Error> MovePetsBetweenPositions(Position newPosition, Position currentPosition)
        {
            if (newPosition.Value < currentPosition.Value)
            {
                var petsToMove = _fosteredPets
                    .Where(p => p.Position.Value >= newPosition.Value
                        && p.Position.Value < currentPosition.Value);

                foreach (var petToMove in petsToMove)
                {
                    var result = petToMove.MoveForward();
                    if (result.IsFailure) return result.Error;
                }
            }
            else if (newPosition.Value > currentPosition.Value)
            {
                var petsToMove = _fosteredPets
                    .Where(p => p.Position.Value > currentPosition.Value
                        && p.Position.Value <= newPosition.Value);

                foreach (var petToMove in petsToMove)
                {
                    var result = petToMove.MoveBack();
                    if (result.IsFailure) return result.Error;
                }
            }

            return Result.Success<Error>();
        }

        private Result<Position, Error> AdjustNewPositionOutOfRange(Position newPosition)
        {
            if (newPosition.Value > _fosteredPets.Count)
            {
                var lastPostionResult = Position.Create(_fosteredPets.Count);
                if (lastPostionResult.IsFailure)
                {
                    return lastPostionResult.Error;
                }
                return lastPostionResult.Value;
            }

            return newPosition;
        }

        public override void Delete()
        {
            base.Delete();
            foreach (var pet in FosteredAnimals)
            {
                pet.Delete();
            }
        }

        public Result<Pet, Error> DeletePet(PetId petId, bool isHardDelete = false)
        {
            var pet = _fosteredPets.FirstOrDefault(a => a.Id == petId);
            if (pet == null)
                return Errors.General.ValueIsInvalid(
                    $"Pet with id {petId.Value} not found in volunteer with id {this.Id.Value}");

            if (isHardDelete)
            {
                _fosteredPets.Remove(pet);
            }
            else
            {
                pet.Delete();
            }

            return pet;
        }

        public override void Restore()
        {
            base.Restore();
            foreach (var pet in FosteredAnimals)
            {
                pet.Restore();
            }
        }
    }
}
