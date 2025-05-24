using FluentValidation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.ValueObjects;

namespace PetFoster.Volunteers.Application.VolunteerManagement.UpdatePersonalInfo;

public sealed record UpdateVolunteerPersonalInfoCommand(Guid Id,
    FullNameDto FullName,
    string Email,
    string PhoneNumber,
    string Description,
    int WorkExperience) : ICommand;

public sealed class UpdateVolunteerPersonalInfoCommandValidator
    : AbstractValidator<UpdateVolunteerPersonalInfoCommand>
{
    public UpdateVolunteerPersonalInfoCommandValidator()
    {
         RuleFor(c => c.Id).NotEmpty()
           .WithError(Errors.General.ValueIsRequired());

         RuleFor(c => c.FullName)
           .MustBeValueObject(f => FullName.Create(f.FirstName, f.LastName, f.Patronymic));

         RuleFor(c => c.Email)
           .MustBeValueObject(Email.Create);

         RuleFor(c => c.PhoneNumber)
           .MustBeValueObject(PhoneNumber.Create);

         RuleFor(c => c.Description)
           .MustBeValueObject(Description.Create);

         RuleFor(c => c.WorkExperience)
           .MustBeValueObject(WorkExperience.Create);
    }
}
