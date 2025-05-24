using FluentValidation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.ValueObjects;

namespace PetFoster.Volunteers.Application.PetManagement.UploadFilesToPet;

public sealed record UploadFilesToPetCommand(Guid VolunteerId,
    Guid PetId, IEnumerable<UploadFileDto> Files) : ICommand;

public sealed class UploadFilesToPetCommandValidator : AbstractValidator<UploadFilesToPetCommand>
{
    public UploadFilesToPetCommandValidator()
    {
         RuleFor(c => c.VolunteerId).NotEmpty()
          .WithError(Errors.General.ValueIsRequired());

         RuleFor(c => c.PetId).NotEmpty()
          .WithError(Errors.General.ValueIsRequired());

         RuleForEach(c => c.Files)
          .MustBeValueObject(f => FilePath.Create(f.FileName));
    }
}
