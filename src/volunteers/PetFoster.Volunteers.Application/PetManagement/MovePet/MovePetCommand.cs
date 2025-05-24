using FluentValidation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.ValueObjects;

namespace PetFoster.Volunteers.Application.PetManagement.MovePet;

public sealed record MovePetCommand(Guid VolunteerId,
    Guid PetId, int NewPosition) : ICommand;

public sealed class MovePetCommandValidator
    : AbstractValidator<MovePetCommand>
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
