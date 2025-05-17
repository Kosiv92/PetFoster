using FluentValidation;
using PetFoster.Application.Validation;
using PetFoster.Core;
using PetFoster.Core.Abstractions;

namespace PetFoster.Volunteers.Application.VolunteerManagement.DeleteVolunteer;

public sealed record DeleteVolunteerCommand(Guid Id) : ICommand;

public sealed class DeleteVolunteerCommandValidator
    : AbstractValidator<DeleteVolunteerCommand>
{
    public DeleteVolunteerCommandValidator()
    {
        _ = RuleFor(c => c.Id).NotEmpty()
            .WithError(Errors.General.ValueIsRequired());
    }
}
