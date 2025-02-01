using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Application.Extensions;
using PetFoster.Application.Volunteers.UpdatePersonalInfo;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Volunteers.UpdateSocialNet
{
    public sealed class UpdateSocialNetHandler
    {
        private readonly IRepository<Volunteer, VolunteerId> _repository;
        private readonly IValidator<UpdateVolunteerSocialNetCommand> _validator;
        private readonly ILogger<UpdateSocialNetHandler> _logger;

        public UpdateSocialNetHandler(IRepository<Volunteer, VolunteerId> repository,
            IValidator<UpdateVolunteerSocialNetCommand> validator,
            ILogger<UpdateSocialNetHandler> logger)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<Guid, ErrorList>> Handle(UpdateVolunteerSocialNetCommand command,
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

            List<SocialNetContact> socialNetContacts = command.SocialNetContactsList
                .Select(s => SocialNetContact.Create(s.SocialNetName, s.AccountName).Value)
                .ToList();

            volunteerForUpdate.UpdateSocialNetContacts(socialNetContacts);

            await _repository.SaveChangesAsync(volunteerForUpdate, cancellationToken);

            return volunteerForUpdate.Id.Value;
        }
    }
}
