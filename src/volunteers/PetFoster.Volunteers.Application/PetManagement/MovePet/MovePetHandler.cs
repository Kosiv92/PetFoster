using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Volunteers.Domain.Entities;

namespace PetFoster.Volunteers.Application.PetManagement.MovePet;

public sealed class MovePetHandler : ICommandHandler<Guid, MovePetCommand>
{
    private readonly IRepository<Volunteer, VolunteerId>? _repository;
    private readonly IValidator<MovePetCommand>? _validator;
    private readonly ILogger<MovePetHandler>? _logger;
    private readonly IUnitOfWork? _unitOfWork;

    public async Task<Result<Guid, ErrorList>> Handle(MovePetCommand command,
        CancellationToken cancellationToken = default)
    {
        FluentValidation.Results.ValidationResult validationResult
            = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        VolunteerId volunteerId = VolunteerId.Create(command.VolunteerId);
        PetId petId = PetId.Create(command.PetId);
        Position newPosition = Position.Create(command.NewPosition).Value;

        Volunteer? volunteer = await _repository
            .GetByIdAsync(volunteerId, cancellationToken);

        if (volunteer == null)
        {
            return Errors.General.NotFound(command.VolunteerId).ToErrorList();
        }

        Pet? pet = volunteer.FosteredAnimals
            .FirstOrDefault(p => p.Id == petId);

        if (pet == null)
        {
            return Errors.General.ValueIsInvalid($"Pet with id {command.PetId} not found in volunteer with id {command.VolunteerId}")
                .ToErrorList();
        }

        System.Data.IDbTransaction transaction = await _unitOfWork.BeginTransaction();

        try
        {
            UnitResult<Error> result = volunteer.MovePet(pet, newPosition);

            if (result.IsFailure)
            {
                return result.Error.ToErrorList();
            }

            await _unitOfWork.SaveChanges(cancellationToken);

            transaction.Commit();

            return volunteer.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError("Throw exception {ExceptionMessage} while try move pet with id {PetId} to volunteer with id {VolunteerId}. Transaction Rollback",
                ex.Message, pet.Id.Value, volunteer.Id.Value);

            transaction.Rollback();

            return Error.Failure("volunteer.move.pet.failure", $"Failed to move pet with id {pet.Id.Value} from volunteer with id {volunteer.Id.Value}")
                .ToErrorList();
        }
    }
}
