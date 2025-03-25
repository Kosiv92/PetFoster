using FluentValidation;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Validation;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Volunteers.MovePet
{
    public sealed record MovePetCommand(Guid VolunteerId, Guid PetId, int NewPosition) : ICommand;

    public sealed class MovePetCommandValidator : AbstractValidator<MovePetCommand>
    {
        public MovePetCommandValidator()
        {
            RuleFor(c => c.VolunteerId).NotEmpty()
                    .WithError(Errors.General.ValueIsRequired());

            RuleFor(c => c.PetId).NotEmpty()
                    .WithError(Errors.General.ValueIsRequired());

            RuleFor(c => c.NewPosition)
                .MustBeValueObject(Position.Create);
        }
    }
}
