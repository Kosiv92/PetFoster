using FluentValidation;
using PetFoster.Application.DTO;
using PetFoster.Application.Validation;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Volunteers.UpdatePersonalInfo
{
    public sealed record UpdateVolunteerPersonalInfoCommand(Guid Id,
        FullNameDto FullName,
        string Email,
        string PhoneNumber,
        string Description,
        int WorkExperience);

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
}
