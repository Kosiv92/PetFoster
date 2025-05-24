using FluentValidation;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;

namespace PetFoster.Volunteers.Application.VolunteerManagement.DeleteVolunteer;

public sealed record DeleteVolunteerCommand(Guid Id) : ICommand;

public sealed class DeleteVolunteerCommandValidator
    : AbstractValidator<DeleteVolunteerCommand>
{
    public DeleteVolunteerCommandValidator()
    {
         RuleFor(c => c.Id).NotEmpty()
           .WithError(Errors.General.ValueIsRequired());
    }
}
