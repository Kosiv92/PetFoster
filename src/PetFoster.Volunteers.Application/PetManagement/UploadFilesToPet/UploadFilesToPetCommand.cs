using FluentValidation;
using PetFoster.Application.Validation;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.ValueObjects;

namespace PetFoster.Volunteers.Application.PetManagement.UploadFilesToPet;

public sealed record UploadFilesToPetCommand(Guid VolunteerId,
    Guid PetId, IEnumerable<UploadFileDto> Files) : ICommand;

public sealed class UploadFilesToPetCommandValidator : AbstractValidator<UploadFilesToPetCommand>
{
    public UploadFilesToPetCommandValidator()
    {
        _ = RuleFor(c => c.VolunteerId).NotEmpty()
            .WithError(Errors.General.ValueIsRequired());

        _ = RuleFor(c => c.PetId).NotEmpty()
            .WithError(Errors.General.ValueIsRequired());

        _ = RuleForEach(c => c.Files)
            .MustBeValueObject(f => FilePath.Create(f.FileName));
    }
}
