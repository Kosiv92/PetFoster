using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Application.Extensions;
using PetFoster.Application.Interfaces;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;

namespace PetFoster.Application.Volunteers.DeletePet
{
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

            var result = volunteer.DeletePet(petId);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to delete (soft) pet with id {PetId} in volunteer with id {VolunteerId}. Get error - {@Error}",
                    command.PetId, command.VolunteerId, result.Error);

                return result.Error.ToErrorList();
            }

            await _repository.SaveChangesAsync(volunteer ,cancellationToken);

            _logger.LogInformation("Successfully delete (soft) pet with id {PetId} in volunteer with id {VolunteerId}",
                    command.PetId, command.VolunteerId);

            return command.PetId;
        }
    }
}
