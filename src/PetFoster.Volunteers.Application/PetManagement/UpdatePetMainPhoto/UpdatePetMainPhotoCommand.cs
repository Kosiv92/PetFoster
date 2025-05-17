using FluentValidation;
using PetFoster.Application.Validation;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.ValueObjects;

namespace PetFoster.Volunteers.Application.PetManagement.UpdatePetMainPhoto;

public sealed record UpdatePetMainPhotoCommand(Guid VolunteerId,
    Guid PetId, string FilePath) : ICommand;

public sealed class UpdatePetMainPhotoCommandValidator
    : AbstractValidator<UpdatePetMainPhotoCommand>
{
    public UpdatePetMainPhotoCommandValidator()
    {
        _ = RuleFor(c => c.VolunteerId).NotEmpty()
            .WithError(Errors.General.ValueIsRequired());

        _ = RuleFor(c => c.PetId).NotEmpty()
            .WithError(Errors.General.ValueIsRequired());

        _ = RuleFor(c => c.FilePath)
        .MustBeValueObject(FilePath.Create);
    }
}
