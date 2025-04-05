using FluentValidation;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Validation;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Species.AddBreed
{
    public sealed record AddBreedCommand(Guid SpecieId, string Name) : ICommand;

    public class AddBreedCommandValidator : AbstractValidator<AddBreedCommand>
    {
        public AddBreedCommandValidator()
        {
            RuleFor(c => c.Name)
                .MustBeValueObject(BreedName.Create);
        }
    }
}
