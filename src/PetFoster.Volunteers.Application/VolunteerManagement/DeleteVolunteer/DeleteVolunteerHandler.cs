using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.Core.Interfaces;
using PetFoster.Volunteers.Domain.Entities;
using PetFoster.Volunteers.Domain.Ids;

namespace PetFoster.Volunteers.Application.VolunteerManagement.DeleteVolunteer;

public sealed class DeleteVolunteerHandler : ICommandHandler<Guid, DeleteVolunteerCommand>
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
        FluentValidation.Results.ValidationResult validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        VolunteerId id = VolunteerId.Create(command.Id);

        Volunteer? volunteerForDelete = await _repository.GetByIdAsync(id, cancellationToken);

        if (volunteerForDelete == null)
        {
            return Errors.General.NotFound(command.Id).ToErrorList();
        }

        volunteerForDelete.Delete();

        await _repository.SaveChangesAsync(volunteerForDelete, cancellationToken);

        _logger.LogInformation("Deleted volunteer {VolunteerFullname} with id {VolunteerId}",
            volunteerForDelete.FullName, volunteerForDelete.Id);

        return (Guid)volunteerForDelete.Id;
    }
}
