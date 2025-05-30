﻿using FluentValidation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.SharedKernel.ValueObjects.Ids;

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
         RuleFor(c => c.FullName)
          .MustBeValueObject(f => FullName.Create(f.FirstName, f.LastName, f.Patronymic));

         RuleFor(c => c.Email)
          .MustBeValueObject(Email.Create);

         RuleFor(c => c.PhoneNumber)
          .MustBeValueObject(PhoneNumber.Create);

         RuleFor(c => c.Description)
          .MustBeValueObject(Description.Create);

         RuleFor(c => c.WorkExpirience)
          .MustBeValueObject(WorkExperience.Create);

         RuleForEach(c => c.AssistanceRequisitesList).ChildRules(a =>
      {
           a.RuleFor(a => a.Description).MustBeValueObject(Description.Create);
           a.RuleFor(a => new { a.Name, Description.Create(a.Description).Value })
          .MustBeValueObject(x => AssistanceRequisites.Create(x.Name, x.Value));
      });

         RuleForEach(c => c.SocialNetContactsList)
          .MustBeValueObject(s => SocialNetContact.Create(s.SocialNetName, s.AccountName));
    }
}
