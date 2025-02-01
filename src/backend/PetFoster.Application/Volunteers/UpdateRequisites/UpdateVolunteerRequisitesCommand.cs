using FluentValidation;
using PetFoster.Application.DTO;
using PetFoster.Application.Validation;
using PetFoster.Application.Volunteers.UpdateRequisites;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Volunteers.UpdateRequisites
{
    public sealed record UpdateVolunteerRequisitesCommand(Guid Id,
        List<AssistanceRequisitesDto> AssistanceRequisitesList);
}

public sealed class UpdateVolunteerRequisitesCommandValidator
    : AbstractValidator<UpdateVolunteerRequisitesCommand>
{
    public UpdateVolunteerRequisitesCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());

        RuleForEach(c => c.AssistanceRequisitesList).ChildRules(a =>
        {
            a.RuleFor(a => a.Description).MustBeValueObject(Description.Create);
            a.RuleFor(a => new { a.Name, Description.Create(a.Description).Value })
            .MustBeValueObject(x => AssistanceRequisites.Create(x.Name, x.Value));
        });
    }
}
