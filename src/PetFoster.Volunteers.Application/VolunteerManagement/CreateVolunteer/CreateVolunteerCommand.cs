using FluentValidation;
using PetFoster.Application.Validation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.ValueObjects;
using PetFoster.Volunteers.Domain.Ids;

namespace PetFoster.Volunteers.Application.VolunteerManagement.CreateVolunteer;

public sealed record CreateVolunteerCommand(VolunteerId id,
    FullNameDto FullName,
    string Email,
    string PhoneNumber,
    string Description,
    int WorkExpirience,
    List<AssistanceRequisitesDto> AssistanceRequisitesList,
    List<SocialNetContactsDto> SocialNetContactsList) : ICommand;

public class CreateModuleCommandValidator : AbstractValidator<CreateVolunteerCommand>
{
    public CreateModuleCommandValidator()
    {
        _ = RuleFor(c => c.FullName)
            .MustBeValueObject(f => FullName.Create(f.FirstName, f.LastName, f.Patronymic));

        _ = RuleFor(c => c.Email)
            .MustBeValueObject(Email.Create);

        _ = RuleFor(c => c.PhoneNumber)
            .MustBeValueObject(PhoneNumber.Create);

        _ = RuleFor(c => c.Description)
            .MustBeValueObject(Description.Create);

        _ = RuleFor(c => c.WorkExpirience)
            .MustBeValueObject(WorkExperience.Create);

        _ = RuleForEach(c => c.AssistanceRequisitesList).ChildRules(a =>
        {
            _ = a.RuleFor(a => a.Description).MustBeValueObject(Description.Create);
            _ = a.RuleFor(a => new { a.Name, Description.Create(a.Description).Value })
            .MustBeValueObject(x => AssistanceRequisites.Create(x.Name, x.Value));
        });

        _ = RuleForEach(c => c.SocialNetContactsList)
            .MustBeValueObject(s => SocialNetContact.Create(s.SocialNetName, s.AccountName));
    }
}
