using FluentValidation;
using PetFoster.Application.Validation;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.ValueObjects;

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
        _ = RuleFor(c => c.Id).NotEmpty()
            .WithError(Errors.General.ValueIsRequired());

        _ = RuleFor(c => c.FullName)
            .MustBeValueObject(f => FullName.Create(f.FirstName, f.LastName, f.Patronymic));

        _ = RuleFor(c => c.Email)
            .MustBeValueObject(Email.Create);

        _ = RuleFor(c => c.PhoneNumber)
            .MustBeValueObject(PhoneNumber.Create);

        _ = RuleFor(c => c.Description)
            .MustBeValueObject(Description.Create);

        _ = RuleFor(c => c.WorkExperience)
            .MustBeValueObject(WorkExperience.Create);
    }
}
