using FluentValidation;
using PetFoster.Application.Validation;
using PetFoster.Core;
using PetFoster.Core.Abstractions;

namespace PetFoster.Species.Application.BreedManagement.DeleteBreed;

public record DeleteBreedCommand(Guid SpecieId, Guid BreedId) : ICommand;

public sealed class DeleteBreedCommandValidator
    : AbstractValidator<DeleteBreedCommand>
{
    public DeleteBreedCommandValidator()
    {
        _ = RuleFor(c => c.SpecieId).NotEmpty()
            .WithError(Errors.General.ValueIsRequired());

        _ = RuleFor(c => c.BreedId).NotEmpty()
            .WithError(Errors.General.ValueIsRequired());
    }
}
