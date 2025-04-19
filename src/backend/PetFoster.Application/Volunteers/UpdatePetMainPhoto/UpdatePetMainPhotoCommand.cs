using FluentValidation;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Validation;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Volunteers.UpdatePetMainPhoto
{
    public sealed record UpdatePetMainPhotoCommand(Guid VolunteerId,
        Guid PetId, string FilePath) : ICommand;

    public sealed class UpdatePetMainPhotoCommandValidator 
        : AbstractValidator<UpdatePetMainPhotoCommand>
    {
        public UpdatePetMainPhotoCommandValidator()
        {
            RuleFor(c => c.VolunteerId).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());

            RuleFor(c => c.PetId).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());

            RuleFor(c => c.FilePath)
            .MustBeValueObject(FilePath.Create);
        }
    }
}
