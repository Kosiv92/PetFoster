using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Application.Extensions;
using PetFoster.Application.Interfaces;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Volunteers.UpdatePersonalInfo
{
    public sealed class UpdateVolunteerPersonalInfoHandler : ICommandHandler<Guid, UpdateVolunteerPersonalInfoCommand>
    {
        private readonly IRepository<Volunteer, VolunteerId> _repository;
        private readonly IValidator<UpdateVolunteerPersonalInfoCommand> _validator;
        private readonly ILogger<UpdateVolunteerPersonalInfoHandler> _logger;

        public UpdateVolunteerPersonalInfoHandler(IRepository<Volunteer, VolunteerId> repository, 
            IValidator<UpdateVolunteerPersonalInfoCommand> validator, 
            ILogger<UpdateVolunteerPersonalInfoHandler> logger)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<Guid,ErrorList>> Handle(UpdateVolunteerPersonalInfoCommand command, 
            CancellationToken cancellationToken = default)
        {
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid)
            {
                return validationResult.ToErrorList();
            }

            var id = VolunteerId.Create(command.Id);

            var volunteerForUpdate = await _repository.GetByIdAsync(id, cancellationToken);

            if (volunteerForUpdate == null)
                return Errors.General.NotFound(command.Id).ToErrorList();

            var fullName = FullName
               .Create(command.FullName.FirstName, command.FullName.LastName, command.FullName.Patronymic)
               .Value;
            var email = Email.Create(command.Email).Value;
            var phoneNumber = PhoneNumber.Create(command.PhoneNumber).Value;
            var description = Description.Create(command.Description).Value;
            var workExepience = WorkExperience.Create(command.WorkExperience).Value;

            volunteerForUpdate.UpdatePersonalInfo(fullName, email, phoneNumber, description, workExepience);

            await _repository.SaveChangesAsync(volunteerForUpdate, cancellationToken);

            return volunteerForUpdate.Id.Value;
        }
    }
}
