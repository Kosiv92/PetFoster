using FluentValidation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;

namespace PetFoster.Volunteers.Application.PetManagement.DeletePet;

public sealed record HardDeletePetCommand(
    Guid VolunteerId,
    Guid PetId) : ICommand;

public sealed class HardDeletePetCommandValidator
    : AbstractValidator<HardDeletePetCommand>
{
    public HardDeletePetCommandValidator()
    {
         RuleFor(c => c.VolunteerId).NotEmpty()
          .WithError(Errors.General.ValueIsRequired());

         RuleFor(c => c.PetId).NotEmpty()
          .WithError(Errors.General.ValueIsRequired());
    }
}
