using FluentValidation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;

namespace PetFoster.Species.Application.SpecieManagement.DeleteSpecie;

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
