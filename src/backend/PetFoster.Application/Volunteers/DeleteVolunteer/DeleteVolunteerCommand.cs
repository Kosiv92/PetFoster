using FluentValidation;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Validation;
using PetFoster.Domain.Shared;

namespace PetFoster.Application.Volunteers.DeleteVolunteer
{
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
}
