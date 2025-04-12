using FluentValidation;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Validation;
using PetFoster.Domain.Shared;

namespace PetFoster.Application.Volunteers.DeletePet
{
    public sealed record HardDeletePetCommand(
        Guid VolunteerId, 
        Guid PetId) : ICommand;
    
    public sealed class HardDeletePetCommandValidator
        : AbstractValidator<HardDeletePetCommand>
    {
        public HardDeletePetCommandValidator()
        {
            RuleFor(c => c.VolunteerId).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());

            RuleFor(c => c.PetId).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());            
        }
    }
}
