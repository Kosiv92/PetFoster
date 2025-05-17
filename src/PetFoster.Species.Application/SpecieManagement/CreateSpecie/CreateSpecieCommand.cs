using FluentValidation;
using PetFoster.Application.Validation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.ValueObjects;

namespace PetFoster.Species.Application.SpecieManagement.CreateSpecie;

public sealed record CreateSpecieCommand(Guid Id, string Name)
    : ICommand;

public class CreateSpecieCommandValidator : AbstractValidator<CreateSpecieCommand>
{
    public CreateSpecieCommandValidator()
    {
        _ = RuleFor(c => c.Name)
            .MustBeValueObject(SpecieName.Create);
    }
}
