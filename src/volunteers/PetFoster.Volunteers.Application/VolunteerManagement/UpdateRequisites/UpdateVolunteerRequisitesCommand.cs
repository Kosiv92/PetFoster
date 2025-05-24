using FluentValidation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.ValueObjects;

namespace PetFoster.Volunteers.Application.VolunteerManagement.UpdateRequisites;

public sealed record UpdateVolunteerRequisitesCommand(Guid Id,
    List<AssistanceRequisitesDto> AssistanceRequisitesList) : ICommand;

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