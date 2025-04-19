using FluentValidation;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Validation;
using PetFoster.Domain.Enums;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Volunteers.UpdatePetInfo
{
    public sealed record UpdatePetInfoCommand(Guid VolunteerId, Guid PetId, string Name, string Description,
        string Health, string Coloration, CharacteristicsDto Characteristics, string OwnerPhoneNumber,
        string BirthDay, Guid SpecieId, Guid BreedId, bool IsCastrated, bool IsVaccinated,
        AddressDto Address, string AssistanceStatus, List<AssistanceRequisitesDto> AssistanceRequisitesList) : ICommand;

    public sealed class UpdatePetInfoCommandValidator : AbstractValidator<UpdatePetInfoCommand>
    {
        public UpdatePetInfoCommandValidator()
        {
            RuleFor(c => c.VolunteerId).NotEmpty()
                    .WithError(Errors.General.ValueIsRequired());

            RuleFor(c => c.PetId).NotEmpty()
                    .WithError(Errors.General.ValueIsRequired());

            RuleFor(c => c.Name)
                .MustBeValueObject(PetName.Create);

            RuleFor(c => c.Description)
                .MustBeValueObject(Description.Create);

            RuleFor(c => c.Health)
                .MustBeValueObject(PetHealth.Create);

            RuleFor(c => c.Coloration)
                .MustBeValueObject(PetColoration.Create);

            RuleFor(c => c.OwnerPhoneNumber)
                .MustBeValueObject(PhoneNumber.Create);

            RuleFor(c => c.Characteristics)
                .MustBeValueObject(f => Characteristics.Create(f.Weight, f.Height));

            RuleFor(c => c.Address)
                .MustBeValueObject(f => Address.Create(f.Region, f.City, f.Street,
                f.HouseNumber, f.ApartmentNumber));

            RuleFor(c => c.SpecieId).NotEmpty()
                    .WithError(Errors.General.ValueIsRequired());

            RuleFor(c => c.BreedId).NotEmpty()
                    .WithError(Errors.General.ValueIsRequired());

            RuleForEach(c => c.AssistanceRequisitesList).ChildRules(a =>
            {
                a.RuleFor(a => a.Description).MustBeValueObject(Description.Create);
                a.RuleFor(a => new { a.Name, Description.Create(a.Description).Value })
                .MustBeValueObject(x => AssistanceRequisites.Create(x.Name, x.Value));
            });

            RuleFor(c => c.BirthDay).Must(IsValidDateOrEmpty)
                .WithError(Errors.General.ValueIsInvalid("Birth date is invalid"));

            RuleFor(c => c.AssistanceStatus).Must(s => Enum.TryParse(s, ignoreCase: true, out AssistanceStatus _))
                .WithError(Errors.General.ValueIsInvalid("Assistance status is invalid"));
        }

        private bool IsValidDateOrEmpty(string inputData)
        {
            if (String.IsNullOrWhiteSpace(inputData)) return true;

            return DateTimeOffset.TryParse(inputData, out _);
        }
    }
}
