﻿using CSharpFunctionalExtensions;
using FluentValidation;
using PetFoster.Application.Extensions;
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
        private readonly IValidator<CreateVolunteerCommand> _validator;

        public CreateVolunteerHandler(IRepository<Volunteer, VolunteerId> repository,
            IValidator<CreateVolunteerCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<Result<Guid, ErrorList>> Handle(CreateVolunteerCommand command, 
            CancellationToken cancellationToken = default)
        {
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid)
            {
                return validationResult.ToErrorList();
            }

            var email = Email.Create(command.Email).Value;
            var phoneNumber = PhoneNumber.Create(command.PhoneNumber).Value;            

            var existVolunteer = await _repository.GetByCriteriaAsync(v => v.Email == email
            || v.PhoneNumber == phoneNumber, cancellationToken);

            if (existVolunteer != null)
            {
                return Errors.Volunteer.AlreadyExist().ToErrorList();
            }
                

            var id = VolunteerId.NewVolunteerId();

            var fullName = FullName
                .Create(command.FullName.FirstName, command.FullName.LastName, command.FullName.Patronymic)
                .Value;
            var description = Description.Create(command.Description).Value;  
            var workExpirience = WorkExperience.Create(command.WorkExpirience).Value;

            List<AssistanceRequisites> assistanceRequisites = command.AssistanceRequisitesList
                .Select(a => AssistanceRequisites
                    .Create(a.Name, Description.Create(a.Description).Value).Value)
                .ToList();
            
            List<SocialNetContact> socialNetContacts = command.SocialNetContactsList
                .Select(s => SocialNetContact.Create(s.SocialNetName, s.AccountName).Value)
                .ToList();            

            var volunteer = new Volunteer(id, fullName, email,
                description, workExpirience, phoneNumber, assistanceRequisites,
                socialNetContacts, new List<Pet>());

            await _repository.AddAsync(volunteer, cancellationToken);
            return (Guid)volunteer.Id;
        }
    }
}
