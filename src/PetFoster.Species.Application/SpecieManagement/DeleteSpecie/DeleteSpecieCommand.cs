using FluentValidation;
using PetFoster.Application.Validation;
using PetFoster.Core;
using PetFoster.Core.Abstractions;

namespace PetFoster.Species.Application.SpecieManagement.DeleteSpecie;

public sealed record DeleteSpecieCommand(Guid Id) : ICommand;

public sealed class DeleteSpecieCommandValidator
    : AbstractValidator<DeleteSpecieCommand>
{
    public DeleteSpecieCommandValidator()
    {
        _ = RuleFor(c => c.Id).NotEmpty()
            .WithError(Errors.General.ValueIsRequired());
    }
}
