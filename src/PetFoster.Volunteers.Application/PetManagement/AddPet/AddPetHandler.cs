using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Database;
using PetFoster.Core.DTO.Specie;
using PetFoster.Core.Extensions;
using PetFoster.Core.Interfaces;
using PetFoster.Core.ValueObjects;
using PetFoster.Species.Domain.Ids;
using PetFoster.Volunteers.Domain.Entities;
using PetFoster.Volunteers.Domain.Enums;
using PetFoster.Volunteers.Domain.Ids;

namespace PetFoster.Volunteers.Application.PetManagement.AddPet
{
    public sealed class AddPetHandler : ICommandHandler<Guid, AddPetCommand>
    {
        private readonly ILogger<AddPetHandler> _logger;
        private readonly IValidator<AddPetCommand> _validator;
        private readonly IRepository<Volunteer, VolunteerId> _volunteerRepository;
        private readonly ISpeciesQueryRepository _speciesQueryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddPetHandler(ILogger<AddPetHandler> logger,
            IValidator<AddPetCommand> validator, [FromKeyedServices("ef")] ISpeciesQueryRepository speciesQueryRepository,
            IRepository<Volunteer, VolunteerId> volunteerRepository, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _validator = validator;
            _speciesQueryRepository = speciesQueryRepository;
            _volunteerRepository = volunteerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid, ErrorList>> Handle(AddPetCommand command,
            CancellationToken cancellationToken = default)
        {
            FluentValidation.Results.ValidationResult validationResult = _validator.Validate(command);

            if (!validationResult.IsValid)
            {
                return validationResult.ToErrorList();
            }

            VolunteerId volunteerId = VolunteerId.Create(command.VolunteerId);

            Volunteer? volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);

            if (volunteer == null)
            {
                return Errors.General.NotFound(volunteerId.Value).ToErrorList();
            }

            SpecieDto specie = await _speciesQueryRepository.GetByIdAsync(new GetSpecieByIdQuery(command.SpecieId), cancellationToken);

            if (specie == null)
            {
                return Errors.General.NotFound().ToErrorList();
            }

            BreedDto? breed = specie.Breeds.FirstOrDefault(b => b.Id == command.BreedId);

            if (breed == null)
            {
                return Errors.General.ValueIsInvalid("Breed not exist in Specie").ToErrorList();
            }

            System.Data.IDbTransaction transaction = await _unitOfWork.BeginTransaction();

            try
            {
                Pet pet = CreatePet(command, SpecieId.Create(specie.Id), BreedId.Create(breed.Id));

                _ = volunteer.AddPet(pet);

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
            DateTimeOffset? birthDay = command.BirthDay.ConvertToDate();

            List<AssistanceRequisites> assistanceRequisites = command.AssistanceRequisitesList
                .Select(a => AssistanceRequisites
                    .Create(a.Name, Description.Create(a.Description).Value).Value)
                .ToList();

            _ = Enum.TryParse(command.AssistanceStatus, ignoreCase: true, out AssistanceStatus status);

            return new Pet(id, name, specieId, description, breedId, coloration, health, address,
                characteristics, phone, command.IsCastrated, birthDay, command.IsVaccinated, status,
                assistanceRequisites);
        }
    }
}
