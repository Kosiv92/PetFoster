using FluentValidation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.ValueObjects;

namespace PetFoster.Volunteers.Application.PetManagement.UpdatePetMainPhoto;

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
