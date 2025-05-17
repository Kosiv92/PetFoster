using FluentValidation;
using PetFoster.Application.Validation;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Volunteers.Domain.Enums;

namespace PetFoster.Volunteers.Application.PetManagement.UpdatePetStatus
{
    public sealed record UpdatePetStatusCommand(Guid VolunteerId, Guid PetId, string AssistanceStatus)
        : ICommand;

    public sealed class UpdatePetStatusCommandValidator
        : AbstractValidator<UpdatePetStatusCommand>
    {
        public UpdatePetStatusCommandValidator()
        {
            _ = RuleFor(c => c.VolunteerId).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());

            _ = RuleFor(c => c.PetId).NotEmpty()
                .WithError(Errors.General.ValueIsRequired());

            _ = RuleFor(c => c.AssistanceStatus).Must(s => Enum.TryParse(s, ignoreCase: true, out AssistanceStatus _))
            .WithError(Errors.General.ValueIsInvalid("Assistance status is invalid"));
        }
    }
}
