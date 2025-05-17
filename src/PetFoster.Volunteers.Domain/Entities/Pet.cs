using CSharpFunctionalExtensions;
using PetFoster.Core;
using PetFoster.Core.Entities;
using PetFoster.Core.ValueObjects;
using PetFoster.Species.Domain.Entities;
using PetFoster.Species.Domain.Ids;
using PetFoster.Volunteers.Domain.Enums;
using PetFoster.Volunteers.Domain.Ids;

namespace PetFoster.Volunteers.Domain.Entities;

public sealed class Pet : SoftDeletableEntity<PetId>
{
    public Pet(PetId id, PetName name, SpecieId specieId, Description description, BreedId breedId,
        PetColoration coloration, PetHealth health, Address address, Characteristics characteristics,
        PhoneNumber ownerPhoneNumber, bool isCastrated, DateTimeOffset? birthDay, bool isVaccinated,
        AssistanceStatus assistanceStatus, List<AssistanceRequisites> assistanceRequisitesList) : base(id)
    {
        Id = id;
        Name = name;
        SpecieId = specieId;
        Description = description;
        BreedId = breedId;
        Coloration = coloration;
        Health = health;
        Address = address;
        Characteristics = characteristics;
        OwnerPhoneNumber = ownerPhoneNumber;
        IsCastrated = isCastrated;
        BirthDay = birthDay;
        IsVaccinated = isVaccinated;
        AssistanceStatus = assistanceStatus;
        _assistanceRequisitesList = assistanceRequisitesList;
        CreatedDate = DateTimeOffset.Now;
    }

    private List<PetFile> _fileList = [];

    public List<AssistanceRequisites> _assistanceRequisitesList
        = [];

    private Pet() { }

    public Volunteer Volunteer { get; private set; }

    public PetName Name { get; private set; }

    public SpecieId SpecieId { get; private set; }

    public Specie Specie { get; private set; }

    public Description Description { get; private set; }

    public BreedId BreedId { get; private set; }

    public Breed Breed { get; private set; }

    public PetColoration Coloration { get; private set; }

    public PetHealth Health { get; private set; }

    public Address Address { get; private set; }

    public Characteristics Characteristics { get; private set; }

    public PhoneNumber OwnerPhoneNumber { get; private set; }

    public bool IsCastrated { get; private set; }

    public DateTimeOffset? BirthDay { get; set; }

    public bool IsVaccinated { get; private set; }

    public Position Position { get; private set; }

    public AssistanceStatus AssistanceStatus { get; private set; }

    public IReadOnlyList<AssistanceRequisites> AssistanceRequisitesList => _assistanceRequisitesList;

    public IReadOnlyList<PetFile> FileList => _fileList;

    public DateTimeOffset CreatedDate { get; private set; }

    public void SetPositionNumber(Position serialNumber)
    {
        Position = serialNumber;
    }

    public void Move(Position newPosition)
    {
        Position = newPosition;
    }

    public UnitResult<Error> MoveForward()
    {
        Result<Position, Error> newPositionResult = Position.Forward();
        if (newPositionResult.IsFailure)
        {
            return newPositionResult.Error;
        }

        Position = newPositionResult.Value;

        return Result.Success<Error>();
    }

    public UnitResult<Error> MoveBack()
    {
        Result<Position, Error> newPositionResult = Position.Back();
        if (newPositionResult.IsFailure)
        {
            return newPositionResult.Error;
        }

        Position = newPositionResult.Value;

        return Result.Success<Error>();
    }

    public void SetVolunteer(Volunteer volunteer)
    {
        Volunteer = volunteer;
    }

    public void UpdateFiles(IEnumerable<PetFile> files)
    {
        _fileList = files.ToList();
    }

    internal UnitResult<Error> UpdateInfo(SpecieId specieId, BreedId breedId, PetName name,
        Description description, PetColoration coloration, PetHealth health, Address address,
        Characteristics characteristics, PhoneNumber phone, DateTimeOffset? birthDay, bool isCastrated,
        bool isVaccinated, AssistanceStatus assistanceStatus, List<AssistanceRequisites> assistanceRequisites)
    {
        SpecieId = specieId;
        BreedId = breedId;
        Name = name;
        Description = description;
        Coloration = coloration;
        Health = health;
        Address = address;
        Characteristics = characteristics;
        OwnerPhoneNumber = phone;
        BirthDay = birthDay;
        IsCastrated = isCastrated;
        IsVaccinated = isVaccinated;
        AssistanceStatus = assistanceStatus;
        _assistanceRequisitesList = assistanceRequisites;

        return Result.Success<Error>();
    }

    internal UnitResult<Error> UpdateAssistanceStatus(AssistanceStatus newAssistanceStatus)
    {
        if (newAssistanceStatus is AssistanceStatus.LookingForHome
            or AssistanceStatus.NeedsHelp)
        {
            AssistanceStatus = newAssistanceStatus;
            return Result.Success<Error>();
        }
        else
        {
            return Errors.General.ValueIsInvalid("Wrong status");
        }
    }

    internal UnitResult<Error> SetMainPhoto(string filePath)
    {
        int index = _fileList.FindIndex(f => f.PathToStorage.Path == filePath);
        if (index >= 0)
        {
            PetFile fileToMove = _fileList[index];
            _fileList.RemoveAt(index);
            _fileList.Insert(0, fileToMove);
        }
        else
        {
            return Errors.General.ValueIsInvalid(
                $"File {filePath} not found in pet with id {Id.Value}");
        }

        return Result.Success<Error>();
    }
}
