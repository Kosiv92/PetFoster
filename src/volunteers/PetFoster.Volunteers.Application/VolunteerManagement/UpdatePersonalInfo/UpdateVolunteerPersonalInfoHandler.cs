using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Volunteers.Domain.Entities;

namespace PetFoster.Volunteers.Application.VolunteerManagement.UpdatePersonalInfo;

public sealed class UpdateVolunteerPersonalInfoHandler
    : ICommandHandler<Guid, UpdateVolunteerPersonalInfoCommand>
{
    private readonly IRepository<Volunteer, VolunteerId> _repository;
    private readonly IValidator<UpdateVolunteerPersonalInfoCommand> _validator;
    private readonly ILogger<UpdateVolunteerPersonalInfoHandler> _logger;

    public UpdateVolunteerPersonalInfoHandler(
        IRepository<Volunteer, VolunteerId> repository,
        IValidator<UpdateVolunteerPersonalInfoCommand> validator,
        ILogger<UpdateVolunteerPersonalInfoHandler> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(UpdateVolunteerPersonalInfoCommand command,
        CancellationToken cancellationToken = default)
    {
        FluentValidation.Results.ValidationResult validationResult = _validator.Validate(command);

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

        FullName fullName = FullName
           .Create(command.FullName.FirstName, command.FullName.LastName, command.FullName.Patronymic)
           .Value;
        Email email = Email.Create(command.Email).Value;
        PhoneNumber phoneNumber = PhoneNumber.Create(command.PhoneNumber).Value;
        Description description = Description.Create(command.Description).Value;
        WorkExperience workExepience = WorkExperience.Create(command.WorkExperience).Value;

        volunteerForUpdate.UpdatePersonalInfo(fullName, email, phoneNumber, description, workExepience);

        await _repository.SaveChangesAsync(volunteerForUpdate, cancellationToken);

        return volunteerForUpdate.Id.Value;
    }
}
