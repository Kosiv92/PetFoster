using FluentValidation;
using PetFoster.Application.Validation;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.ValueObjects;
using PetFoster.Volunteers.Domain.Enums;

namespace PetFoster.Volunteers.Application.PetManagement.UpdatePetInfo;

public sealed record UpdatePetInfoCommand(Guid VolunteerId, Guid PetId, string Name, string Description,
    string Health, string Coloration, CharacteristicsDto Characteristics, string OwnerPhoneNumber,
    string BirthDay, Guid SpecieId, Guid BreedId, bool IsCastrated, bool IsVaccinated,
    AddressDto Address, string AssistanceStatus, List<AssistanceRequisitesDto> AssistanceRequisitesList) : ICommand;

public sealed class UpdatePetInfoCommandValidator : AbstractValidator<UpdatePetInfoCommand>
{
    public UpdatePetInfoCommandValidator()
    {
        _ = RuleFor(c => c.VolunteerId).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());

        _ = RuleFor(c => c.PetId).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());

        _ = RuleFor(c => c.Name)
            .MustBeValueObject(PetName.Create);

        _ = RuleFor(c => c.Description)
            .MustBeValueObject(Description.Create);

        _ = RuleFor(c => c.Health)
            .MustBeValueObject(PetHealth.Create);

        _ = RuleFor(c => c.Coloration)
            .MustBeValueObject(PetColoration.Create);

        _ = RuleFor(c => c.OwnerPhoneNumber)
            .MustBeValueObject(PhoneNumber.Create);

        _ = RuleFor(c => c.Characteristics)
            .MustBeValueObject(f => Characteristics.Create(f.Weight, f.Height));

        _ = RuleFor(c => c.Address)
            .MustBeValueObject(f => Address.Create(f.Region, f.City, f.Street,
            f.HouseNumber, f.ApartmentNumber));

        _ = RuleFor(c => c.SpecieId).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());

        _ = RuleFor(c => c.BreedId).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());

        _ = RuleForEach(c => c.AssistanceRequisitesList).ChildRules(a =>
        {
            _ = a.RuleFor(a => a.Description).MustBeValueObject(Description.Create);
            _ = a.RuleFor(a => new { a.Name, Description.Create(a.Description).Value })
            .MustBeValueObject(x => AssistanceRequisites.Create(x.Name, x.Value));
        });

        _ = RuleFor(c => c.BirthDay).Must(IsValidDateOrEmpty)
            .WithError(Errors.General.ValueIsInvalid("Birth date is invalid"));

        _ = RuleFor(c => c.AssistanceStatus).Must(s => Enum.TryParse(s, ignoreCase: true, out AssistanceStatus _))
            .WithError(Errors.General.ValueIsInvalid("Assistance status is invalid"));
    }

    private bool IsValidDateOrEmpty(string inputData)
    {
        return string.IsNullOrWhiteSpace(inputData) || DateTimeOffset.TryParse(inputData, out _);
    }
}
