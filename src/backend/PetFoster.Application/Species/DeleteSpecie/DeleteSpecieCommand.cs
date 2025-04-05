using FluentValidation;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Validation;
using PetFoster.Domain.Shared;

namespace PetFoster.Application.Species.DeleteSpecie
{
    public sealed record DeleteSpecieCommand(Guid Id) : ICommand;

    public sealed class DeleteSpecieCommandValidator
        : AbstractValidator<DeleteSpecieCommand>
    {
        public DeleteSpecieCommandValidator()
        {
            RuleFor(c => c.Id).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());
        }
    }
}
