using FluentValidation;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Validation;
using PetFoster.Application.Volunteers.UpdateSocialNet;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Volunteers.UpdateSocialNet
{
    public sealed record UpdateVolunteerSocialNetCommand(Guid Id,
        List<SocialNetContactsDto> SocialNetContactsList) : ICommand;
}

public sealed class UpdateVolunteerSocialNetCommandValidator
    : AbstractValidator<UpdateVolunteerSocialNetCommand>
{
    public UpdateVolunteerSocialNetCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());

        RuleForEach(c => c.SocialNetContactsList)
            .MustBeValueObject(s => SocialNetContact.Create(s.SocialNetName, s.AccountName));
    }
}
