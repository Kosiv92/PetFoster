using FluentValidation;
using PetFoster.Application.Validation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.ValueObjects;

namespace PetFoster.Species.Application.BreedManagement.AddBreed;

public sealed record AddBreedCommand(Guid SpecieId, string Name) : ICommand;

public class AddBreedCommandValidator : AbstractValidator<AddBreedCommand>
{
    public AddBreedCommandValidator()
    {
        _ = RuleFor(c => c.Name)
            .MustBeValueObject(BreedName.Create);
    }
}
