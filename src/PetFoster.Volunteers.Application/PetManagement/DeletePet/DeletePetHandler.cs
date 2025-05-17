using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.Core.Interfaces;
using PetFoster.Volunteers.Application.Interfaces;
using PetFoster.Volunteers.Domain.Entities;
using PetFoster.Volunteers.Domain.Ids;

namespace PetFoster.Volunteers.Application.PetManagement.DeletePet;

public sealed class DeletePetHandler : ICommandHandler<Guid, DeletePetCommand>
{
    private readonly IRepository<Volunteer, VolunteerId> _repository;
    private readonly IVolunteersQueryRepository _queryRepository;
    private readonly IValidator<DeletePetCommand> _validator;
    private readonly ILogger<DeletePetHandler> _logger;

    public DeletePetHandler(IRepository<Volunteer, VolunteerId> repository,
        IVolunteersQueryRepository queryRepository,
        IValidator<DeletePetCommand> validator,
        ILogger<DeletePetHandler> logger)
    {
        _repository = repository;
        _queryRepository = queryRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(DeletePetCommand command, CancellationToken cancellationToken = default)
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

        Result<Pet, Error> result = volunteer.DeletePet(petId);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to delete (soft) pet with id {PetId} in volunteer with id {VolunteerId}. Get error - {@Error}",
                command.PetId, command.VolunteerId, result.Error);

            return result.Error.ToErrorList();
        }

        await _repository.SaveChangesAsync(volunteer, cancellationToken);

        _logger.LogInformation("Successfully delete (soft) pet with id {PetId} in volunteer with id {VolunteerId}",
                command.PetId, command.VolunteerId);

        return command.PetId;
    }
}
