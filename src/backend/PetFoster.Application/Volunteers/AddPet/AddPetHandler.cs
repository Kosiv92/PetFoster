﻿using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFoster.Application.Database;
using PetFoster.Application.Extensions;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Species.GetSpecie;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Enums;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Volunteers.AddPet
{
    public sealed class AddPetHandler : ICommandHandler<Guid, AddPetCommand>
    {
        private const string BUCKET_NAME = "files";

        private readonly IFileProvider _fileProvider;
        private readonly ILogger<AddPetHandler> _logger;
        private readonly IValidator<AddPetCommand> _validator;        
        private readonly IRepository<Volunteer, VolunteerId> _volunteerRepository;
        private readonly ISpeciesQueryRepository _speciesQueryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddPetHandler(IFileProvider fileProvider, ILogger<AddPetHandler> logger, 
            IValidator<AddPetCommand> validator, [FromKeyedServices("ef")] ISpeciesQueryRepository speciesQueryRepository, 
            IRepository<Volunteer, VolunteerId> volunteerRepository, IUnitOfWork unitOfWork)
        {
            _fileProvider = fileProvider;
            _logger = logger;
            _validator = validator;
            _speciesQueryRepository = speciesQueryRepository;
            _volunteerRepository = volunteerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid, ErrorList>> Handle(AddPetCommand command,
            CancellationToken cancellationToken = default)
        {
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid)
            {
                return validationResult.ToErrorList();
            }

            VolunteerId volunteerId = VolunteerId.Create(command.VolunteerId);

            var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);

            if (volunteer == null)
            {
                return Errors.General.NotFound(volunteerId.Value).ToErrorList();
            }

            var specie = await _speciesQueryRepository.GetByIdAsync(new GetSpecieByIdQuery(command.SpecieId), cancellationToken);
                
            if(specie == null)
            {
                return Errors.General.NotFound().ToErrorList();
            }

            var breed = specie.Breeds.FirstOrDefault(b => b.Id == command.BreedId);
            
            if (breed == null)
            {
                return Errors.General.ValueIsInvalid("Breed not exist in Specie").ToErrorList();
            }

            var transaction = await _unitOfWork.BeginTransaction();

            try
            {
                var pet = CreatePet(command, SpecieId.Create(specie.Id), BreedId.Create(breed.Id));

                volunteer.AddPet(pet);

                await _unitOfWork.SaveChanges(cancellationToken);                               

                transaction.Commit();

                return pet.Id.Value;
            }
            catch (Exception ex) 
            {
                _logger.LogError("Throw exception {ExceptionMessage} while try add pet to volunteer with id {VolunteerId}. Transaction Rollback", 
                    ex.Message, volunteer.Id.Value);

                transaction.Rollback();

                return Error.Failure("volunteer.add.pet.failure", $"Failed to add pet to volunteer with id {volunteer.Id.Value}")
                    .ToErrorList();
            }
        }

        private Pet CreatePet(AddPetCommand command, SpecieId specieId, BreedId breedId)
        {
            PetId id = PetId.NewPetId();
            PetName name = PetName.Create(command.Name).Value;
            Description description = Description.Create(command.Description).Value;
            PetColoration coloration = PetColoration.Create(command.Coloration).Value;
            PetHealth health = PetHealth.Create(command.Health).Value;
            Address address = Address.Create(command.Address.Region, command.Address.City,
                command.Address.Street, command.Address.HouseNumber, command.Address.ApartmentNumber).Value;
            Characteristics characteristics = Characteristics.Create(command.Characteristics.Weight,
                command.Characteristics.Height).Value;
            PhoneNumber phone = PhoneNumber.Create(command.OwnerPhoneNumber).Value;
            DateTimeOffset? birthDay = GetBirthDay(command.BirthDay);

            List<AssistanceRequisites> assistanceRequisites = command.AssistanceRequisitesList
                .Select(a => AssistanceRequisites
                    .Create(a.Name, Description.Create(a.Description).Value).Value)
                .ToList();

            Enum.TryParse(command.AssistanceStatus, ignoreCase: true, out AssistanceStatus status);

            return new Pet(id, name, specieId, description, breedId, coloration, health, address,
                characteristics, phone, command.IsCastrated, birthDay, command.IsVaccinated, status,
                assistanceRequisites);
        }

        private DateTimeOffset? GetBirthDay(string inputData)
        {
            if(String.IsNullOrWhiteSpace(inputData)) return null;
            
            DateTimeOffset.TryParse(inputData, out DateTimeOffset birthDay);
            
            return birthDay;
        }
    }
}
