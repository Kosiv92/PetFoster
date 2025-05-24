using FluentValidation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel.ValueObjects;

namespace PetFoster.Species.Application.SpecieManagement.CreateSpecie;

public sealed record CreateSpecieCommand(Guid Id, string Name)
    : ICommand;

public class CreateSpecieCommandValidator : AbstractValidator<CreateSpecieCommand>
{
    public CreateSpecieCommandValidator()
    {
         RuleFor(c => c.Name)
           .MustBeValueObject(SpecieName.Create);
    }
}
