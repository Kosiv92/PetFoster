using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Application.Extensions;
using PetFoster.Application.Interfaces;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Enums;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Volunteers.AddPet
{
    public sealed class AddPetHandler
    {
        private readonly IFileProvider _fileProvider;
        private readonly ILogger<AddPetHandler> _logger;
        private readonly IValidator<AddPetCommand> _validator;
        private readonly IRepository<Specie, SpecieId> _specieRepository;
        private readonly IRepository<Volunteer, VolunteerId> _volunteerRepository;

        public AddPetHandler(IFileProvider fileProvider, ILogger<AddPetHandler> logger, 
            IValidator<AddPetCommand> validator, IRepository<Specie, SpecieId> specieRepository, 
            IRepository<Volunteer, VolunteerId> volunteerRepository)
        {
            _fileProvider = fileProvider;
            _logger = logger;
            _validator = validator;
            _specieRepository = specieRepository;
            _volunteerRepository = volunteerRepository;
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

            var specie = await _specieRepository
                .GetByCriteriaAsync(s => s.Name.Value == command.Name, cancellationToken);

            if(specie == null)
            {
                return Errors.General.NotFound().ToErrorList();
            }

            var breed = specie.Breeds.FirstOrDefault(b => b.Name.Value == command.Name);
            
            if (breed == null)
            {
                return Errors.General.NotFound().ToErrorList();
            }

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

            var pet = new Pet(id, name, specie, description, breed, coloration, health, address, 
                characteristics, phone, command.IsCastrated, birthDay, command.IsVaccinated, status, 
                assistanceRequisites);

            volunteer.AddPet(pet);

            return pet.Id.Value;
        }

        private DateTimeOffset? GetBirthDay(string inputData)
        {
            if(String.IsNullOrWhiteSpace(inputData)) return null;
            
            DateTimeOffset.TryParse(inputData, out DateTimeOffset birthDay);

            return birthDay;
        }
    }
}
