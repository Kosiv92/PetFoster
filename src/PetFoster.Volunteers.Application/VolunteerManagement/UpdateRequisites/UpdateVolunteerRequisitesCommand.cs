using FluentValidation;
using PetFoster.Application.Validation;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.ValueObjects;

namespace PetFoster.Volunteers.Application.VolunteerManagement.UpdateRequisites;

public sealed record UpdateVolunteerRequisitesCommand(Guid Id,
    List<AssistanceRequisitesDto> AssistanceRequisitesList) : ICommand;

public sealed class UpdateVolunteerRequisitesCommandValidator
    : AbstractValidator<UpdateVolunteerRequisitesCommand>
{
    public UpdateVolunteerRequisitesCommandValidator()
    {
        _ = RuleFor(c => c.Id).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());

        _ = RuleForEach(c => c.AssistanceRequisitesList).ChildRules(a =>
        {
            _ = a.RuleFor(a => a.Description).MustBeValueObject(Description.Create);
            _ = a.RuleFor(a => new { a.Name, Description.Create(a.Description).Value })
            .MustBeValueObject(x => AssistanceRequisites.Create(x.Name, x.Value));
        });
    }
}



