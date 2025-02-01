using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Application.Extensions;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Volunteers.UpdateRequisites
{
    public sealed class UpdateRequisitesHandler
    {
        private readonly IRepository<Volunteer, VolunteerId> _repository;
        private readonly IValidator<UpdateVolunteerRequisitesCommand> _validator;
        private readonly ILogger<UpdateRequisitesHandler> _logger;

        public UpdateRequisitesHandler(IRepository<Volunteer, VolunteerId> repository,
            IValidator<UpdateVolunteerRequisitesCommand> validator,
            ILogger<UpdateRequisitesHandler> logger)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<Guid, ErrorList>> Handle(UpdateVolunteerRequisitesCommand command,
            CancellationToken cancellationToken = default)
        {
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid)
            {
                return validationResult.ToErrorList();
            }

            var id = VolunteerId.Create(command.Id);

            var volunteerForUpdate = await _repository.GetByIdAsync(id, cancellationToken);

            if (volunteerForUpdate == null)
                return Errors.General.NotFound(command.Id).ToErrorList();

            List<AssistanceRequisites> assistanceRequisites = command.AssistanceRequisitesList
                .Select(a => AssistanceRequisites
                    .Create(a.Name, Description.Create(a.Description).Value).Value)
                .ToList();

            volunteerForUpdate.UpdateRequisites(assistanceRequisites);

            await _repository.SaveChangesAsync(volunteerForUpdate, cancellationToken);

            _logger.LogInformation("The list of assistance requisites of volunteer {VolunteerFullname} with id {VolunteerId} has been updated",
            volunteerForUpdate.FullName, volunteerForUpdate.Id);

            return volunteerForUpdate.Id.Value;
        }
    }
}
