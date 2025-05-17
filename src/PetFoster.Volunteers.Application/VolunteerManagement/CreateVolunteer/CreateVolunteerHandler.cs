using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.Core.Interfaces;
using PetFoster.Core.ValueObjects;
using PetFoster.Volunteers.Domain.Entities;
using PetFoster.Volunteers.Domain.Ids;

namespace PetFoster.Volunteers.Application.VolunteerManagement.CreateVolunteer;

public sealed class CreateVolunteerHandler : ICommandHandler<Guid, CreateVolunteerCommand>
{
    private readonly IRepository<Volunteer, VolunteerId> _repository;
    private readonly IValidator<CreateVolunteerCommand> _validator;
    private readonly ILogger<CreateVolunteerHandler> _logger;

    public CreateVolunteerHandler(IRepository<Volunteer, VolunteerId> repository,
        IValidator<CreateVolunteerCommand> validator,
        ILogger<CreateVolunteerHandler> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(CreateVolunteerCommand command,
        CancellationToken cancellationToken = default)
    {
        FluentValidation.Results.ValidationResult validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        Email email = Email.Create(command.Email).Value;
        PhoneNumber phoneNumber = PhoneNumber.Create(command.PhoneNumber).Value;

        Volunteer? sameIdVolunteer = await _repository.GetByIdAsync(command.id, cancellationToken);

        if (sameIdVolunteer != null)
        {
            return Errors.Volunteer.IdAlreadyExist().ToErrorList();
        }

        Volunteer? existVolunteer = await _repository.GetByCriteriaAsync(v => v.Email == email
        || v.PhoneNumber == phoneNumber, cancellationToken);

        if (existVolunteer != null)
        {
            return Errors.Volunteer.AlreadyExist().ToErrorList();
        }

        VolunteerId id = VolunteerId.Create(command.id);

        FullName fullName = FullName
            .Create(command.FullName.FirstName, command.FullName.LastName, command.FullName.Patronymic)
            .Value;
        Description description = Description.Create(command.Description).Value;
        WorkExperience workExpirience = WorkExperience.Create(command.WorkExpirience).Value;

        List<AssistanceRequisites> assistanceRequisites = command.AssistanceRequisitesList
            .Select(a => AssistanceRequisites
            .Create(a.Name, Description.Create(a.Description).Value).Value)
            .ToList();

        List<SocialNetContact> socialNetContacts = command.SocialNetContactsList
            .Select(s => SocialNetContact.Create(s.SocialNetName, s.AccountName).Value)
            .ToList();

        Volunteer volunteer = new(id, fullName, email,
            description, workExpirience, phoneNumber, assistanceRequisites,
            socialNetContacts);

        await _repository.AddAsync(volunteer, cancellationToken);

        _logger.LogInformation("Created volunteer {VolunteerFullname} with id {VolunteerId}",
            volunteer.FullName.ToString(), volunteer.Id.Value);

        return (Guid)volunteer.Id;
    }
}
