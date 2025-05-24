using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Volunteers.Domain.Entities;

namespace PetFoster.Volunteers.Application.PetManagement.UpdatePetMainPhoto;

public sealed class UpdatePetMainPhotoHandler
    : ICommandHandler<Guid, UpdatePetMainPhotoCommand>
{
    private readonly ILogger<UpdatePetMainPhotoHandler> _logger;
    private readonly IValidator<UpdatePetMainPhotoCommand> _validator;
    private readonly IRepository<Volunteer, VolunteerId> _repository;

    public UpdatePetMainPhotoHandler(ILogger<UpdatePetMainPhotoHandler> logger,
        IValidator<UpdatePetMainPhotoCommand> validator,
        IRepository<Volunteer, VolunteerId> repository)
    {
        _logger = logger;
        _validator = validator;
        _repository = repository;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        UpdatePetMainPhotoCommand command,
        CancellationToken cancellationToken = default)
    {
        FluentValidation.Results.ValidationResult validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        VolunteerId id = VolunteerId.Create(command.VolunteerId);

        Volunteer? volunteer = await _repository.GetByIdAsync(id, cancellationToken);

        if (volunteer == null)
        {
            return Errors.General.NotFound(command.VolunteerId).ToErrorList();
        }

        PetId petId = PetId.Create(command.PetId);

        UnitResult<Error> result = volunteer.UpdatePetMainPhoto(petId, command.FilePath);

        if (result.IsFailure)
        {
            _logger.LogError(
                "Failed to update main photo for pet with id {PetId} in volunteer with id {VolunteerId}. Get error - {@Error}",
                command.PetId, command.VolunteerId, result.Error);

            return result.Error.ToErrorList();
        }

        try
        {
            await _repository.SaveChangesAsync(volunteer, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Throw exception '{Exception}' while update main photo for pet with id {PetId} in volunteer with id {VolunteerId}",
                ex.Message, command.PetId, command.VolunteerId);

            return Error.Failure("volunteer.update.main.photo.pet.failure"
                , $"Failed to update main photo for pet with id {command.PetId} from volunteer with id {volunteer.Id.Value}")
                .ToErrorList();
        }

        _logger.LogInformation("Successfully update main photo for pet with id {PetId} in volunteer with id {VolunteerId}",
                command.PetId, command.VolunteerId);

        return petId.Value;
    }
}
