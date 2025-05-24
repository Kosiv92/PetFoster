using FluentValidation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;

namespace PetFoster.Species.Application.BreedManagement.DeleteBreed;

public record DeleteBreedCommand(Guid SpecieId, Guid BreedId) : ICommand;

public sealed class DeleteBreedCommandValidator
    : AbstractValidator<DeleteBreedCommand>
{
    public DeleteBreedCommandValidator()
    {
         RuleFor(c => c.SpecieId).NotEmpty()
           .WithError(Errors.General.ValueIsRequired());

         RuleFor(c => c.BreedId).NotEmpty()
           .WithError(Errors.General.ValueIsRequired());
    }
}
