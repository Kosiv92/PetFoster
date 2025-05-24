using FluentValidation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;

namespace PetFoster.Volunteers.Application.PetManagement.DeletePet;

public sealed record DeletePetCommand(
    Guid VolunteerId,
    Guid PetId) : ICommand;
public sealed class DeletePetCommandValidator
    : AbstractValidator<DeletePetCommand>
{
    public DeletePetCommandValidator()
    {
         RuleFor(c => c.VolunteerId).NotEmpty()
          .WithError(Errors.General.ValueIsRequired());

         RuleFor(c => c.PetId).NotEmpty()
          .WithError(Errors.General.ValueIsRequired());
    }
}
