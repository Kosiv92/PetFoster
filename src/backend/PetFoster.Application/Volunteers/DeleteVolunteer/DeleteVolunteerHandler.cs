using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Application.Extensions;
using PetFoster.Application.Volunteers.DeleteVolunteer;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;

namespace PetFoster.Application.Volunteers.CreateVolunteer
{
    public sealed class DeleteVolunteerHandler
    {
        private readonly IRepository<Volunteer, VolunteerId> _repository;
        private readonly IValidator<DeleteVolunteerCommand> _validator;
        private readonly ILogger<DeleteVolunteerHandler> _logger;

        public DeleteVolunteerHandler(IRepository<Volunteer, VolunteerId> repository,
            IValidator<DeleteVolunteerCommand> validator, 
            ILogger<DeleteVolunteerHandler> logger)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<Guid, ErrorList>> Handle(DeleteVolunteerCommand command, 
            CancellationToken cancellationToken = default)
        {
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid)
            {
                return validationResult.ToErrorList();
            }

            var id = VolunteerId.Create(command.Id);

            var volunteerForDelete = await _repository.GetByIdAsync(id, cancellationToken);

            if (volunteerForDelete == null)
                return Errors.General.NotFound(command.Id).ToErrorList();

            volunteerForDelete.Delete();

            await _repository.SaveChangesAsync(volunteerForDelete, cancellationToken);            

            _logger.LogInformation("Deleted volunteer {VolunteerFullname} with id {VolunteerId}",
                volunteerForDelete.FullName, volunteerForDelete.Id);

            return (Guid)volunteerForDelete.Id;
        }
    }
}
