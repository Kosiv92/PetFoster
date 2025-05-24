using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Specie;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.Enums;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Species.Contracts;
using PetFoster.Volunteers.Domain.Entities;

namespace PetFoster.Volunteers.Application.PetManagement.UpdatePetInfo;

public sealed class UpdatePetInfoHandler
    : ICommandHandler<Guid, UpdatePetInfoCommand>
{
    private readonly ILogger<UpdatePetInfoHandler> _logger;
    private readonly IValidator<UpdatePetInfoCommand> _validator;
    private readonly IRepository<Volunteer, VolunteerId> _repository;
    private readonly ISpeciesContract _speciesContract;

    public UpdatePetInfoHandler(ILogger<UpdatePetInfoHandler> logger,
        IValidator<UpdatePetInfoCommand> validator,
        IRepository<Volunteer, VolunteerId> repository,
        ISpeciesContract speciesContract)
    {
        _logger = logger;
        _validator = validator;
        _repository = repository;
        _speciesContract = speciesContract;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        UpdatePetInfoCommand command,
        CancellationToken cancellationToken = default)
    {
        FluentValidation.Results.ValidationResult validationResult
            = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        VolunteerId volunteerId = VolunteerId.Create(command.VolunteerId);

        Volunteer? volunteer = await _repository
            .GetByIdAsync(volunteerId, cancellationToken);

        if (volunteer == null)
        {
            return Errors.General.NotFound(volunteerId.Value).ToErrorList();
        }

        SpecieDto specie = await _speciesContract
            .GetSpecieById(SpecieId.Create(command.SpecieId),
            cancellationToken);

        if (specie == null)
        {
            return Errors.General.NotFound().ToErrorList();
        }

        BreedDto? breed = specie.Breeds
            .FirstOrDefault(b => b.Id == command.BreedId);

        if (breed == null)
        {
            return Errors.General.ValueIsInvalid("Breed not exist in Specie")
                .ToErrorList();
        }

        PetId petId = PetId.Create(command.PetId);
        SpecieId specieId = SpecieId.Create(specie.Id);
        BreedId breedId = BreedId.Create(breed.Id);
        PetName name = PetName.Create(command.Name).Value;
        Description description = Description.Create(command.Description).Value;
        PetColoration coloration = PetColoration.Create(command.Coloration).Value;
        PetHealth health = PetHealth.Create(command.Health).Value;
        Address address = Address.Create(command.Address.Region, command.Address.City,
            command.Address.Street, command.Address.HouseNumber, command.Address.ApartmentNumber).Value;
        Characteristics characteristics = Characteristics.Create(command.Characteristics.Weight,
            command.Characteristics.Height).Value;
        PhoneNumber phone = PhoneNumber.Create(command.OwnerPhoneNumber).Value;
        DateTimeOffset? birthDay = command.BirthDay.ConvertToDate();

        List<AssistanceRequisites> assistanceRequisites = command.AssistanceRequisitesList
            .Select(a => AssistanceRequisites
                .Create(a.Name, Description.Create(a.Description).Value).Value)
            .ToList();

         Enum.TryParse(command.AssistanceStatus, ignoreCase: true, out AssistanceStatus status);

        UnitResult<Error> result = volunteer.UpdatePetInfo(petId, specieId, breedId, name, description, coloration, health, address,
        characteristics, phone, birthDay, command.IsCastrated, command.IsVaccinated, status, assistanceRequisites);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to update info for pet with id {PetId} in volunteer with id {VolunteerId}. Get error - {@Error}",
                command.PetId, command.VolunteerId, result.Error);

            return result.Error.ToErrorList();
        }

        try
        {
            await _repository.SaveChangesAsync(volunteer, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Throw exception '{Exception}' while update info for pet with id {PetId} in volunteer with id {VolunteerId}",
                ex.Message, command.PetId, command.VolunteerId);

            return Error.Failure("volunteer.update.info.pet.failure"
                , $"Failed to update pet with id {command.PetId} from volunteer with id {volunteer.Id.Value}")
                .ToErrorList();
        }

        _logger.LogInformation("Successfully update info for pet with id {PetId} in volunteer with id {VolunteerId}",
                command.PetId, command.VolunteerId);

        return petId.Value;
    }
}
