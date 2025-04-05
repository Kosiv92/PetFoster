using FluentValidation;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Validation;
using PetFoster.Domain.Shared;

namespace PetFoster.Application.Species.DeleteBreed
{
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
}
