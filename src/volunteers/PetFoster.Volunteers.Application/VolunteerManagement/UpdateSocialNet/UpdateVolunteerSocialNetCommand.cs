using FluentValidation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.ValueObjects;

namespace PetFoster.Volunteers.Application.VolunteerManagement.UpdateSocialNet;

public sealed record UpdateVolunteerSocialNetCommand(Guid Id,
    List<SocialNetContactsDto> SocialNetContactsList) : ICommand;

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