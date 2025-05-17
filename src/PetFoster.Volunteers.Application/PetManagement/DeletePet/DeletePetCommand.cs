using FluentValidation;
using PetFoster.Application.Validation;
using PetFoster.Core;
using PetFoster.Core.Abstractions;

namespace PetFoster.Volunteers.Application.PetManagement.DeletePet;

public sealed record DeletePetCommand(
    Guid VolunteerId,
    Guid PetId) : ICommand;
public sealed class DeletePetCommandValidator
    : AbstractValidator<DeletePetCommand>
{
    public DeletePetCommandValidator()
    {
        _ = RuleFor(c => c.VolunteerId).NotEmpty()
            .WithError(Errors.General.ValueIsRequired());

        _ = RuleFor(c => c.PetId).NotEmpty()
            .WithError(Errors.General.ValueIsRequired());
    }
}
