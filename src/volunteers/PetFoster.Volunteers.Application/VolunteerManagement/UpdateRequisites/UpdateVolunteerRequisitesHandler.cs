using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Volunteers.Domain.Entities;

namespace PetFoster.Volunteers.Application.VolunteerManagement.UpdateRequisites;

public sealed class UpdateVolunteerRequisitesHandler
    : ICommandHandler<Guid, UpdateVolunteerRequisitesCommand>
{
    private readonly IRepository<Volunteer, VolunteerId> _repository;
    private readonly IValidator<UpdateVolunteerRequisitesCommand> _validator;
    private readonly ILogger<UpdateVolunteerRequisitesHandler> _logger;

    public UpdateVolunteerRequisitesHandler(IRepository<Volunteer, VolunteerId> repository,
        IValidator<UpdateVolunteerRequisitesCommand> validator,
        ILogger<UpdateVolunteerRequisitesHandler> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateVolunteerRequisitesCommand command,
        CancellationToken cancellationToken = default)
    {
        FluentValidation.Results.ValidationResult validationResult
            = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        VolunteerId id = VolunteerId.Create(command.Id);

        Volunteer? volunteerForUpdate = await _repository.GetByIdAsync(id, cancellationToken);

        if (volunteerForUpdate == null)
        {
            return Errors.General.NotFound(command.Id).ToErrorList();
        }

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
