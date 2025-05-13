using AutoFixture;
using Bogus;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Volunteers.AddPet;
using PetFoster.Application.Volunteers.CreateVolunteer;
using PetFoster.Application.Volunteers.UpdatePetInfo;
using PetFoster.Application.Volunteers.UploadFilesToPet;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Enums;
using PetFoster.Domain.Ids;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.IntegrateTests
{
    public static class FixtureExtensions
    {
        public static CreateVolunteerCommand CreateCreateVolunteerCommand(
            this IFixture fixture, Guid volunteerId)
        {
            var faker = new Faker();

            return fixture.Build<CreateVolunteerCommand>()
                .With(c => c.id, VolunteerId.Create(volunteerId))     
                .With(c => c.Email, () => faker.Internet.Email())
                .With(c => c.PhoneNumber, () => faker.Phone.PhoneNumber("###########"))
                .Create();
        }

        public static AddPetCommand CreateAddPetCommandComand(this IFixture fixture, 
            Guid volunteerId, Guid specieId, Guid breedId)
        {
            var faker = new Faker();

            var name = fixture.Create<string>();
            var description = fixture.Create<string>();
            var health = fixture.Create<string>();
            var coloration = fixture.Create<string>();
            var characterisitcs = fixture.Create<CharacteristicsDto>();
            var ownerPhone = faker.Phone.PhoneNumber("###########");
            var birthDay = faker.Date.Recent(3).ToString("dd.MM.yyyy");
            var isCastrated = fixture.Create<bool>();
            var isVaccinated = fixture.Create<bool>();
            var assistanceStatus = fixture.Create<AssistanceStatus>().ToString();
            var assistanceRequisites = fixture.Create<List<AssistanceRequisitesDto>>();

            var address = fixture.Build<AddressDto>()
                .With(c => c.Region, () => faker.Random.String2(1, Address.MAX_REGION_LENGTH))
                .With(c => c.City, () => faker.Random.String2(1, Address.MAX_CITY_LENGTH))
                .With(c => c.Street, () => faker.Random.String2(1, Address.MAX_STREET_LENGTH))
                .With(c => c.HouseNumber, () => faker.Random.String2(1, Address.MAX_HOUSE_LENGTH))
                .With(c => c.ApartmentNumber, () => faker.Random.String2(1, Address.MAX_APARTMENT_LENGTH))
                .Create();

            return new AddPetCommand(volunteerId, name, description, health, coloration,
                characterisitcs, ownerPhone, birthDay, specieId, breedId, isCastrated,
                isVaccinated, address, assistanceStatus, assistanceRequisites);
        }

        public static UpdatePetInfoCommand CreateUpdatePetCommand(this IFixture fixture,
            Guid volunteerId, Guid petId, Guid specieId, Guid breedId)
        {
            var faker = new Faker();

            var name = fixture.Create<string>();
            var description = fixture.Create<string>();
            var health = fixture.Create<string>();
            var coloration = fixture.Create<string>();
            var characterisitcs = fixture.Create<CharacteristicsDto>();
            var ownerPhone = faker.Phone.PhoneNumber("###########");
            var birthDay = faker.Date.Recent(3).ToString("dd.MM.yyyy");
            var isCastrated = fixture.Create<bool>();
            var isVaccinated = fixture.Create<bool>();
            var assistanceStatus = fixture.Create<AssistanceStatus>().ToString();
            var assistanceRequisites = fixture.Create<List<AssistanceRequisitesDto>>();

            var address = fixture.Build<AddressDto>()
                .With(c => c.Region, () => faker.Random.String2(1, Address.MAX_REGION_LENGTH))
                .With(c => c.City, () => faker.Random.String2(1, Address.MAX_CITY_LENGTH))
                .With(c => c.Street, () => faker.Random.String2(1, Address.MAX_STREET_LENGTH))
                .With(c => c.HouseNumber, () => faker.Random.String2(1, Address.MAX_HOUSE_LENGTH))
                .With(c => c.ApartmentNumber, () => faker.Random.String2(1, Address.MAX_APARTMENT_LENGTH))
                .Create();

            return new UpdatePetInfoCommand(volunteerId, petId, name, 
                description, health, coloration, characterisitcs, ownerPhone, 
                birthDay, specieId, breedId, isCastrated, isVaccinated, address, 
                assistanceStatus, assistanceRequisites);
        }

        public static Volunteer CreateVolunteer(
            this IFixture fixture, Guid volunteerId)
        {
            var faker = new Faker();

            List<AssistanceRequisites> assistanceRequisites = new List<AssistanceRequisites>();
            List<SocialNetContact> socialNetContacts = new List<SocialNetContact>();

            for(int i = 0; i < 2; i++)
            {
                var assistanceRequisite = AssistanceRequisites.Create(
                    fixture.Create<string>(),
                    Description.Create(fixture.Create<string>()).Value).Value;

                assistanceRequisites.Add(assistanceRequisite);

                var socialNetContact = SocialNetContact.Create(
                    fixture.Create<string>(), 
                    fixture.Create<string>()).Value;

                socialNetContacts.Add(socialNetContact);
            }

            var id = VolunteerId.Create(volunteerId);
            var fullName = FullName.Create(faker.Name.FirstName(), faker.Name.LastName(), faker.Name.FirstName()).Value;
            var email = Email.Create(faker.Internet.Email()).Value;
            var description = Description.Create(fixture.Create<string>()).Value;
            var workExpirience = WorkExperience.Create(fixture.Create<int>()).Value;
            var phoneNumber = PhoneNumber.Create(faker.Phone.PhoneNumber("###########")).Value;

            return new Volunteer(id, fullName, email, 
                description, workExpirience, phoneNumber,
                assistanceRequisites, socialNetContacts);
        }

        public static Pet CreatePet(this IFixture fixture, 
            Guid petId, Guid specieId, Guid breedId, string? petName, string? color)
        {
            var faker = new Faker();

            var id = PetId.Create(petId);
            var name = petName != null ? PetName.Create(petName).Value 
                : PetName.Create(faker.Random.String2(PetName.MIN_NAME_LENGTH, PetName.MAX_NAME_LENGTH))
                .Value;
            var specieIdObject = SpecieId.Create(specieId);
            var breedIdObject = BreedId.Create(breedId);
            var description = Description.Create(fixture.Create<string>()).Value;
            var coloration = color != null ? PetColoration.Create(color).Value 
                : PetColoration.Create(faker.Random.String2(3, PetColoration.MAX_NAME_LENGTH)).Value;
            var health = PetHealth.Create(faker.Random.String2(3, PetHealth.MAX_HEALTH_LENGTH)).Value;
            var address = Address.Create(
                faker.Random.String2(1, Address.MAX_REGION_LENGTH),
                faker.Random.String2(1, Address.MAX_CITY_LENGTH),
                faker.Random.String2(1, Address.MAX_STREET_LENGTH),
                faker.Random.String2(1, Address.MAX_HOUSE_LENGTH),
                faker.Random.String2(1, Address.MAX_APARTMENT_LENGTH)).Value;               

            var characterisitcs = Characteristics.Create(
                fixture.Create<double>(), 
                fixture.Create<double>()).Value;

            var phoneNumber = PhoneNumber.Create(faker.Phone.PhoneNumber("###########")).Value;

            var isCastrated = fixture.Create<bool>();
            var isVaccinated = fixture.Create<bool>();

            var birthDay = new DateTimeOffset(faker.Date.Recent(3));

            var assistanceStatus = fixture.Create<AssistanceStatus>();

            List<AssistanceRequisites> assistanceRequisites = new List<AssistanceRequisites>();

            for (int i = 0; i < 2; i++)
            {
                var assistanceRequisite = AssistanceRequisites.Create(
                    fixture.Create<string>(),
                    Description.Create(fixture.Create<string>()).Value).Value;

                assistanceRequisites.Add(assistanceRequisite);
            }

            return new Pet(id, name, specieIdObject, description, breedIdObject, coloration, 
                health, address, characterisitcs, phoneNumber, isCastrated, birthDay, isVaccinated, assistanceStatus,
                assistanceRequisites);
        }

        public static Specie CreateSpecie(this IFixture fixture, Guid specieId)
        {
            var id = SpecieId.Create(specieId);
            var specieName = SpecieName.Create(fixture.Create<string>()).Value;

            return new Specie(id, specieName);
        }

        public static Breed CreateBreed(this IFixture fixture, Guid breedId)
        {
            var id = BreedId.Create(breedId);
            var breedName = BreedName.Create(fixture.Create<string>()).Value;

            return new Breed(id, breedName);
        }

        public static UploadFilesToPetCommand CreateUploadFilesToPetCommand(
            this IFixture fixture, Guid volunteerId, Guid petId, int filesCount)
        {
            fixture.Register<Stream>(() => new MemoryStream());

            List<UploadFileDto> ulpoadFilesDto = new List<UploadFileDto>();
            for (int i = 0; i < filesCount; i++)
            {
                ulpoadFilesDto.Add(new UploadFileDto(
                    fixture.Create<Stream>(),
                    fixture.Create<string>()));
            }

            return new UploadFilesToPetCommand(volunteerId, petId, ulpoadFilesDto);
        }
    }
}
