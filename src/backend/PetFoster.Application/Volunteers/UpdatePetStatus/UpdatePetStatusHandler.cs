using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Application.Extensions;
using PetFoster.Application.Interfaces;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Enums;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;

namespace PetFoster.Application.Volunteers.UpdatePetStatus
{
    public sealed class UpdatePetStatusHandler 
        : ICommandHandler<Guid, UpdatePetStatusCommand>
    {
        private readonly IRepository<Volunteer, VolunteerId> _repository;
        private readonly IValidator<UpdatePetStatusCommand> _validator;
        private readonly ILogger<UpdatePetStatusHandler> _logger;

        public UpdatePetStatusHandler(IRepository<Volunteer, VolunteerId> repository, 
            IValidator<UpdatePetStatusCommand> validator, ILogger<UpdatePetStatusHandler> logger)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<Guid, ErrorList>> Handle(
            UpdatePetStatusCommand command, 
            CancellationToken cancellationToken = default)
        {
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid)
            {
                return validationResult.ToErrorList();
            }

            var id = VolunteerId.Create(command.VolunteerId);

            var volunteer = await _repository.GetByIdAsync(id, cancellationToken);

            if (volunteer == null)
                return Errors.General.NotFound(command.VolunteerId).ToErrorList();

            var petId = PetId.Create(command.PetId);
            Enum.TryParse(command.AssistanceStatus, ignoreCase: true, out AssistanceStatus assistanceStatus);

            var result = volunteer.UpdatePetAssistanceStatus(petId, assistanceStatus);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to update assistance status ({AssistanceStatusCode}) for pet with id {PetId} in volunteer with id {VolunteerId}. Get error - {@Error}", 
                    command.AssistanceStatus, command.PetId, command.VolunteerId, result.Error);
                return result.Error.ToErrorList();
            }

            _logger.LogInformation("Successfully update assistance status ({AssistanceStatusCode}) for pet with id {PetId} in volunteer with id {VolunteerId}",
                    command.AssistanceStatus, command.PetId, command.VolunteerId);

            return petId.Value;
        }
    }
}
