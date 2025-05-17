using FluentValidation;
using PetFoster.Application.Validation;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.ValueObjects;

namespace PetFoster.Volunteers.Application.PetManagement.MovePet;

public sealed record MovePetCommand(Guid VolunteerId, Guid PetId, int NewPosition) : ICommand;

public sealed class MovePetCommandValidator : AbstractValidator<MovePetCommand>
{
    public MovePetCommandValidator()
    {
        _ = RuleFor(c => c.VolunteerId).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());

        _ = RuleFor(c => c.PetId).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());

        _ = RuleFor(c => c.NewPosition)
            .MustBeValueObject(Position.Create);
    }
}
