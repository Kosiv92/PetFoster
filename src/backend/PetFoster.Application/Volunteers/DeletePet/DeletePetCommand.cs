using FluentValidation;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Validation;
using PetFoster.Domain.Shared;

namespace PetFoster.Application.Volunteers.DeletePet
{
    public sealed record DeletePetCommand(
        Guid VolunteerId, 
        Guid PetId) : ICommand;
    public sealed class DeletePetCommandValidator
        : AbstractValidator<DeletePetCommand>
    {
        public DeletePetCommandValidator()
        {
            RuleFor(c => c.VolunteerId).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());

            RuleFor(c => c.PetId).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());            
        }
    }
}
