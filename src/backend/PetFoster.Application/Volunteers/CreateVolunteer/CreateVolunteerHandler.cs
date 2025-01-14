using CSharpFunctionalExtensions;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Volunteers.CreateVolunteer
{
    public sealed class CreateVolunteerHandler
    {
        private readonly IRepository<Volunteer, VolunteerId> _repository;

        public CreateVolunteerHandler(IRepository<Volunteer, VolunteerId> repository)
        {
            _repository = repository;
        }

        public async Task<Result<Guid, Error>> Handle(CreateVolunteerCommand command, 
            CancellationToken cancellationToken = default)
        {
            var id = VolunteerId.NewVolunteerId();

            var fullNameResult = FullName.Create(command.FirstName, command.LastName, command.Patronymic);
            if (fullNameResult.IsFailure) return fullNameResult.Error;

            var emailResult = Email.Create(command.Email);
            if (emailResult.IsFailure) return emailResult.Error;

            var descriptionResult = Description.Create(command.Description);
            if (descriptionResult.IsFailure) return descriptionResult.Error;

            var workExpirienceResult = WorkExperience.Create(command.WorkExpirience);
            if (workExpirienceResult.IsFailure) return workExpirienceResult.Error;

            var phoneNumberResult = PhoneNumber.Create(command.PhoneNumber);
            if (phoneNumberResult.IsFailure) return phoneNumberResult.Error;

            List<AssistanceRequisites> assistanceRequisites = new List<AssistanceRequisites>();

            foreach (var requisites in command.AssistanceRequisitesList)
            {
                var asistanceDescriptionResult = Description.Create(requisites.Description);
                if (asistanceDescriptionResult.IsFailure) return asistanceDescriptionResult.Error;

                var assistanceRequisitesResult = AssistanceRequisites.Create(requisites.Name, asistanceDescriptionResult.Value);
                if (assistanceRequisitesResult.IsFailure) return assistanceRequisitesResult.Error;

                assistanceRequisites.Add(assistanceRequisitesResult.Value);
            }

            List<SocialNetContact> socialNetContacts = new List<SocialNetContact>();

            foreach (var contact in command.SocialNetContactsList)
            {
                var socialNetContactResult = SocialNetContact.Create(contact.SocialNetName, contact.AccountName);
                if (socialNetContactResult.IsFailure) return socialNetContactResult.Error;

                socialNetContacts.Add(socialNetContactResult.Value);
            }

            var volunteer = new Volunteer(id, fullNameResult.Value, emailResult.Value,
                descriptionResult.Value, workExpirienceResult.Value, phoneNumberResult.Value, assistanceRequisites,
                socialNetContacts, new List<Pet>());

            await _repository.AddAsync(volunteer, cancellationToken);
            return (Guid)volunteer.Id;
        }
    }
}
