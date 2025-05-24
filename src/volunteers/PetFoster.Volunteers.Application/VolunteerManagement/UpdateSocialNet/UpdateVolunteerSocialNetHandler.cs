using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Volunteers.Domain.Entities;

namespace PetFoster.Volunteers.Application.VolunteerManagement.UpdateSocialNet;

public sealed class UpdateVolunteerSocialNetHandler
    : ICommandHandler<Guid, UpdateVolunteerSocialNetCommand>
{
    private readonly IRepository<Volunteer, VolunteerId> _repository;
    private readonly IValidator<UpdateVolunteerSocialNetCommand> _validator;
    private readonly ILogger<UpdateVolunteerSocialNetHandler> _logger;

    public UpdateVolunteerSocialNetHandler(IRepository<Volunteer, VolunteerId> repository,
        IValidator<UpdateVolunteerSocialNetCommand> validator,
        ILogger<UpdateVolunteerSocialNetHandler> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateVolunteerSocialNetCommand command,
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

        List<SocialNetContact> socialNetContacts = command.SocialNetContactsList
            .Select(s => SocialNetContact.Create(s.SocialNetName, s.AccountName).Value)
            .ToList();

        volunteerForUpdate.UpdateSocialNetContacts(socialNetContacts);

        await _repository.SaveChangesAsync(volunteerForUpdate, cancellationToken);

        _logger.LogInformation("The list of social networks of volunteer {VolunteerFullname} with id {VolunteerId} has been updated",
        volunteerForUpdate.FullName, volunteerForUpdate.Id);

        return volunteerForUpdate.Id.Value;
    }
}
