using FluentValidation;
using PetFoster.Application.DTO;
using PetFoster.Application.Validation;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Volunteers.UploadFilesToPet
{
    public sealed record UploadFilesToPetCommand(Guid VolunteerId,
        Guid PetId, IEnumerable<UploadFileDto> Files);

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
}
