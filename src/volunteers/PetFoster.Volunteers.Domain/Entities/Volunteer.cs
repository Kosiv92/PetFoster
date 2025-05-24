using CSharpFunctionalExtensions;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.Entities;
using PetFoster.SharedKernel.Enums;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.SharedKernel.ValueObjects.Ids;

namespace PetFoster.Volunteers.Domain.Entities;

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

    private List<AssistanceRequisites> _assistanceRequisites = [];

    private List<SocialNetContact> _socialNetContacts = [];

    private readonly List<Pet> _fosteredPets = [];

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
        .Count(a => a.AssistanceStatus == AssistanceStatus.FoundHome) ?? 0;

    public int LookingForHomeAnimals => FosteredAnimals?
        .Count(a => a.AssistanceStatus == AssistanceStatus.LookingForHome) ?? 0;

    public int NeedsHelpAnimals => FosteredAnimals?
        .Count(a => a.AssistanceStatus == AssistanceStatus.NeedsHelp) ?? 0;

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
    {
        _socialNetContacts = socialNetContacts.ToList();
    }

    public void UpdateRequisites(IEnumerable<AssistanceRequisites> requisites)
    {
        _assistanceRequisites = requisites.ToList();
    }

    public UnitResult<Error> AddPet(Pet pet)
    {
        Result<Position, Error> serialNumberResult = Position.Create(_fosteredPets.Count + 1);
        if (serialNumberResult.IsFailure)
        {
            return serialNumberResult.Error;
        }

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
        Pet? pet = _fosteredPets.FirstOrDefault(a => a.Id == petId);
        return pet == null
            ? (UnitResult<Error>)Errors.General.ValueIsInvalid(
                $"Pet with id {petId.Value} not found in volunteer with id {Id.Value}")
            : pet.UpdateInfo(specieId, breedId, name, description, coloration, health, address,
        characteristics, phone, birthDay, isCastrated, isVaccinated, assistanceStatus, assistanceRequisites);
    }

    public UnitResult<Error> UpdatePetAssistanceStatus(PetId petId,
        AssistanceStatus assistanceStatus)
    {
        Pet? pet = _fosteredPets.FirstOrDefault(a => a.Id == petId);
        return pet == null
            ? (UnitResult<Error>)Errors.General.ValueIsInvalid(
                $"Pet with id {petId.Value} not found in volunteer with id {Id.Value}")
            : pet.UpdateAssistanceStatus(assistanceStatus);
    }

    public UnitResult<Error> UpdatePetMainPhoto(PetId petId,
        string filePath)
    {
        Pet? pet = _fosteredPets.FirstOrDefault(a => a.Id == petId);
        return pet == null
            ? (UnitResult<Error>)Errors.General.ValueIsInvalid(
                $"Pet with id {petId.Value} not found in volunteer with id {Id.Value}")
            : pet.SetMainPhoto(filePath);
    }

    public UnitResult<Error> MovePet(Pet pet, Position newPosition)
    {
        if (pet.Volunteer != this)
        {
            return Errors.General.ValueIsInvalid("The pet does not belong to this volunteer");
        }

        Position currentPosition = pet.Position;

        if (currentPosition == newPosition || _fosteredPets.Count == 1)
        {
            return Result.Success<Error>();
        }

        Result<Position, Error> adjustedPosition = AdjustNewPositionOutOfRange(newPosition);
        if (adjustedPosition.IsFailure)
        {
            return adjustedPosition.Error;
        }

        newPosition = adjustedPosition.Value;

        UnitResult<Error> movePetsResult = MovePetsBetweenPositions(newPosition, currentPosition);
        if (movePetsResult.IsFailure)
        {
            return movePetsResult.Error;
        }

        pet.Move(newPosition);

        int uniqCounts = _fosteredPets.Select(p => p.Position.Value).Distinct().Count();
        return uniqCounts != _fosteredPets.Count
            ? (UnitResult<Error>)Error.Failure("nonuniq.pet.position", "Non-unique animal positions at the volunteer")
            : Result.Success<Error>();
    }

    public UnitResult<Error> MovePetToStart(Pet pet)
    {
        return MovePet(pet, Position.First);
    }

    public UnitResult<Error> MovePetToEnd(Pet pet)
    {
        Position lastPosition = Position.Create(_fosteredPets.Count).Value;
        return MovePet(pet, lastPosition);
    }

    private UnitResult<Error> MovePetsBetweenPositions(Position newPosition, Position currentPosition)
    {
        if (newPosition.Value < currentPosition.Value)
        {
            IEnumerable<Pet> petsToMove = _fosteredPets
                .Where(p => p.Position.Value >= newPosition.Value
                    && p.Position.Value < currentPosition.Value);

            foreach (Pet? petToMove in petsToMove)
            {
                UnitResult<Error> result = petToMove.MoveForward();
                if (result.IsFailure)
                {
                    return result.Error;
                }
            }
        }
        else if (newPosition.Value > currentPosition.Value)
        {
            IEnumerable<Pet> petsToMove = _fosteredPets
                .Where(p => p.Position.Value > currentPosition.Value
                    && p.Position.Value <= newPosition.Value);

            foreach (Pet? petToMove in petsToMove)
            {
                UnitResult<Error> result = petToMove.MoveBack();
                if (result.IsFailure)
                {
                    return result.Error;
                }
            }
        }

        return Result.Success<Error>();
    }

    private Result<Position, Error> AdjustNewPositionOutOfRange(Position newPosition)
    {
        if (newPosition.Value > _fosteredPets.Count)
        {
            Result<Position, Error> lastPostionResult = Position.Create(_fosteredPets.Count);
            return lastPostionResult.IsFailure ? (Result<Position, Error>)lastPostionResult.Error : (Result<Position, Error>)lastPostionResult.Value;
        }

        return newPosition;
    }

    public override void Delete()
    {
        base.Delete();
        foreach (Pet pet in FosteredAnimals)
        {
            pet.Delete();
        }
    }

    public Result<Pet, Error> DeletePet(PetId petId, bool isHardDelete = false)
    {
        Pet? pet = _fosteredPets.FirstOrDefault(a => a.Id == petId);
        if (pet == null)
        {
            return Errors.General.ValueIsInvalid(
                $"Pet with id {petId.Value} not found in volunteer with id {Id.Value}");
        }

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
        foreach (Pet pet in FosteredAnimals)
        {
            pet.Restore();
        }
    }
}
