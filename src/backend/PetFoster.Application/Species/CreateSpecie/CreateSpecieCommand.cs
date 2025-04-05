using FluentValidation;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Validation;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Species.CreateSpecie
{
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
}
