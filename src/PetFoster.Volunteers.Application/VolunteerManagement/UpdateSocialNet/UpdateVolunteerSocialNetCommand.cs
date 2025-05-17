using FluentValidation;
using PetFoster.Application.Validation;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.ValueObjects;
using PetFoster.Volunteers.Application.VolunteerManagement.UpdateSocialNet;

namespace PetFoster.Volunteers.Application.VolunteerManagement.UpdateSocialNet;

public sealed record UpdateVolunteerSocialNetCommand(Guid Id,
    List<SocialNetContactsDto> SocialNetContactsList) : ICommand;

public sealed class UpdateVolunteerSocialNetCommandValidator
: AbstractValidator<UpdateVolunteerSocialNetCommand>
{
    public UpdateVolunteerSocialNetCommandValidator()
    {
        _ = RuleFor(c => c.Id).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());

        _ = RuleForEach(c => c.SocialNetContactsList)
            .MustBeValueObject(s => SocialNetContact.Create(s.SocialNetName, s.AccountName));
    }
}


