using AutoFixture;
using Bogus;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.SharedKernel.Enums;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.Species.Domain.Entities;
using PetFoster.Volunteers.Application.PetManagement.AddPet;
using PetFoster.Volunteers.Application.PetManagement.UpdatePetInfo;
using PetFoster.Volunteers.Application.PetManagement.UploadFilesToPet;
using PetFoster.Volunteers.Application.VolunteerManagement.CreateVolunteer;
using PetFoster.Volunteers.Domain.Entities;

namespace PetFoster.IntegrateTests;

public static class FixtureExtensions
{
    public static CreateVolunteerCommand CreateCreateVolunteerCommand(
        this IFixture fixture, Guid volunteerId)
    {
        Faker faker = new();

        return fixture.Build<CreateVolunteerCommand>()
            .With(c => c.id, VolunteerId.Create(volunteerId))
            .With(c => c.Email, () => faker.Internet.Email())
            .With(c => c.PhoneNumber, () => faker.Phone.PhoneNumber("###########"))
            .Create();
    }

    public static AddPetCommand CreateAddPetCommandComand(this IFixture fixture,
        Guid volunteerId, Guid specieId, Guid breedId)
    {
        Faker faker = new();

        string name = fixture.Create<string>();
        string description = fixture.Create<string>();
        string health = fixture.Create<string>();
        string coloration = fixture.Create<string>();
        CharacteristicsDto characterisitcs = fixture.Create<CharacteristicsDto>();
        string ownerPhone = faker.Phone.PhoneNumber("###########");
        string birthDay = faker.Date.Recent(3).ToString("dd.MM.yyyy");
        bool isCastrated = fixture.Create<bool>();
        bool isVaccinated = fixture.Create<bool>();
        string assistanceStatus = fixture.Create<AssistanceStatus>().ToString();
        List<AssistanceRequisitesDto> assistanceRequisites = fixture.Create<List<AssistanceRequisitesDto>>();

        AddressDto address = fixture.Build<AddressDto>()
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
        Faker faker = new();

        string name = fixture.Create<string>();
        string description = fixture.Create<string>();
        string health = fixture.Create<string>();
        string coloration = fixture.Create<string>();
        CharacteristicsDto characterisitcs = fixture.Create<CharacteristicsDto>();
        string ownerPhone = faker.Phone.PhoneNumber("###########");
        string birthDay = faker.Date.Recent(3).ToString("dd.MM.yyyy");
        bool isCastrated = fixture.Create<bool>();
        bool isVaccinated = fixture.Create<bool>();
        string assistanceStatus = fixture.Create<AssistanceStatus>().ToString();
        List<AssistanceRequisitesDto> assistanceRequisites = fixture.Create<List<AssistanceRequisitesDto>>();

        AddressDto address = fixture.Build<AddressDto>()
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
        Faker faker = new();

        List<AssistanceRequisites> assistanceRequisites = [];
        List<SocialNetContact> socialNetContacts = [];

        for (int i = 0; i < 2; i++)
        {
            AssistanceRequisites assistanceRequisite = AssistanceRequisites.Create(
                fixture.Create<string>(),
                Description.Create(fixture.Create<string>()).Value).Value;

            assistanceRequisites.Add(assistanceRequisite);

            SocialNetContact socialNetContact = SocialNetContact.Create(
                fixture.Create<string>(),
                fixture.Create<string>()).Value;

            socialNetContacts.Add(socialNetContact);
        }

        VolunteerId id = VolunteerId.Create(volunteerId);
        FullName fullName = FullName.Create(faker.Name.FirstName(), faker.Name.LastName(), faker.Name.FirstName()).Value;
        Email email = Email.Create(faker.Internet.Email()).Value;
        Description description = Description.Create(fixture.Create<string>()).Value;
        WorkExperience workExpirience = WorkExperience.Create(fixture.Create<int>()).Value;
        PhoneNumber phoneNumber = PhoneNumber.Create(faker.Phone.PhoneNumber("###########")).Value;

        return new Volunteer(id, fullName, email,
            description, workExpirience, phoneNumber,
            assistanceRequisites, socialNetContacts);
    }

    public static Pet CreatePet(this IFixture fixture,
        Guid petId, Guid specieId, Guid breedId, string? petName, string? color)
    {
        Faker faker = new();

        PetId id = PetId.Create(petId);
        PetName name = petName != null ? PetName.Create(petName).Value
            : PetName.Create(faker.Random.String2(PetName.MIN_NAME_LENGTH, PetName.MAX_NAME_LENGTH))
            .Value;
        SpecieId specieIdObject = SpecieId.Create(specieId);
        BreedId breedIdObject = BreedId.Create(breedId);
        Description description = Description.Create(fixture.Create<string>()).Value;
        PetColoration coloration = color != null ? PetColoration.Create(color).Value
            : PetColoration.Create(faker.Random.String2(3, PetColoration.MAX_NAME_LENGTH)).Value;
        PetHealth health = PetHealth.Create(faker.Random.String2(3, PetHealth.MAX_HEALTH_LENGTH)).Value;
        Address address = Address.Create(
            faker.Random.String2(1, Address.MAX_REGION_LENGTH),
            faker.Random.String2(1, Address.MAX_CITY_LENGTH),
            faker.Random.String2(1, Address.MAX_STREET_LENGTH),
            faker.Random.String2(1, Address.MAX_HOUSE_LENGTH),
            faker.Random.String2(1, Address.MAX_APARTMENT_LENGTH)).Value;

        Characteristics characterisitcs = Characteristics.Create(
            fixture.Create<double>(),
            fixture.Create<double>()).Value;

        PhoneNumber phoneNumber = PhoneNumber.Create(faker.Phone.PhoneNumber("###########")).Value;

        bool isCastrated = fixture.Create<bool>();
        bool isVaccinated = fixture.Create<bool>();

        DateTimeOffset birthDay = new(faker.Date.Recent(3));

        AssistanceStatus assistanceStatus = fixture.Create<AssistanceStatus>();

        List<AssistanceRequisites> assistanceRequisites = [];

        for (int i = 0; i < 2; i++)
        {
            AssistanceRequisites assistanceRequisite = AssistanceRequisites.Create(
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
        SpecieId id = SpecieId.Create(specieId);
        SpecieName specieName = SpecieName.Create(fixture.Create<string>()).Value;

        return new Specie(id, specieName);
    }

    public static Breed CreateBreed(this IFixture fixture, Guid breedId)
    {
        BreedId id = BreedId.Create(breedId);
        BreedName breedName = BreedName.Create(fixture.Create<string>()).Value;

        return new Breed(id, breedName);
    }

    public static UploadFilesToPetCommand CreateUploadFilesToPetCommand(
        this IFixture fixture, Guid volunteerId, Guid petId, int filesCount)
    {
        fixture.Register<Stream>(() => new MemoryStream());

        List<UploadFileDto> ulpoadFilesDto = [];
        for (int i = 0; i < filesCount; i++)
        {
            ulpoadFilesDto.Add(new UploadFileDto(
                fixture.Create<Stream>(),
                fixture.Create<string>()));
        }

        return new UploadFilesToPetCommand(volunteerId, petId, ulpoadFilesDto);
    }
}
