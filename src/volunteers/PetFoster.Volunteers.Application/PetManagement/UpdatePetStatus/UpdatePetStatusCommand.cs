using FluentValidation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.Enums;

namespace PetFoster.Volunteers.Application.PetManagement.UpdatePetStatus
{
    public sealed record UpdatePetStatusCommand(Guid VolunteerId, Guid PetId, string AssistanceStatus)
        : ICommand;

    public sealed class UpdatePetStatusCommandValidator
        : AbstractValidator<UpdatePetStatusCommand>
    {
        public UpdatePetStatusCommandValidator()
        {
             RuleFor(c => c.VolunteerId).NotEmpty()
              .WithError(Errors.General.ValueIsRequired());

             RuleFor(c => c.PetId).NotEmpty()
              .WithError(Errors.General.ValueIsRequired());

             RuleFor(c => c.AssistanceStatus).Must(s => Enum.TryParse(s, ignoreCase: true, out AssistanceStatus _))
          .WithError(Errors.General.ValueIsInvalid("Assistance status is invalid"));
        }
    }
}
