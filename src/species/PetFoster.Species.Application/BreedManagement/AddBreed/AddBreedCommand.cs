using FluentValidation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel.ValueObjects;

namespace PetFoster.Species.Application.BreedManagement.AddBreed;

public sealed record AddBreedCommand(Guid SpecieId, string Name) : ICommand;

public class AddBreedCommandValidator : AbstractValidator<AddBreedCommand>
{
    public AddBreedCommandValidator()
    {
         RuleFor(c => c.Name)
           .MustBeValueObject(BreedName.Create);
    }
}
