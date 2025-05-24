using FluentAssertions;
using PetFoster.SharedKernel.Enums;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Volunteers.Domain.Entities;

namespace PetFoster.UnitTests.Domain;

public class VolunteerTests
{
    public const string PHONE_NUMBER = "89999999999";
    public const string DESCRIPTION = "Any description";

    [Fact]
    public void Add_Pet_With_Empty_Pets_Return_Success_Result()
    {
        //Arrange
        Volunteer volunteer = CreateVolunteerWithPets(0);

        Pet pet = CreatePet();         

        //Act
        var result = volunteer.AddPet(pet);

        //Assert
         result.IsSuccess.Should().BeTrue();
         result.IsFailure.Should().BeFalse();
         pet.Position.Should().Be(Position.First);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    public void Add_Pet_With_Other_Pets_Return_Success_Result(int petsCount)
    {
        //Arrange            
        Volunteer volunteer = CreateVolunteerWithPets(petsCount);

        Pet pet = CreatePet();

        //Act
        var result = volunteer.AddPet(pet);

        //Assert
         result.IsSuccess.Should().BeTrue();
         result.IsFailure.Should().BeFalse();
         pet.Position.Should()
            .BeEquivalentTo(Position.Create(volunteer.FosteredAnimals.Count).Value);
    }

    [Fact]
    public void Move_Pet_Should_Not_Move_When_Pet_Already_At_New_Position()
    {
        //Arrange            
        Volunteer volunteer = CreateVolunteerWithPets(3);

        Pet firstPet = volunteer.FosteredAnimals[0];
        Pet secondPet = volunteer.FosteredAnimals[1];
        Pet thirdPet = volunteer.FosteredAnimals[2];

        Pet petToMove = volunteer.FosteredAnimals[1];
        Position position = Position.Create(2).Value;

        //Act
        var result = volunteer.MovePet(petToMove, position);

        //Assert
         result.IsSuccess.Should().BeTrue();
         result.IsFailure.Should().BeFalse();

         firstPet.Position.Value.Should().Be(1);
         secondPet.Position.Value.Should().Be(2);
         thirdPet.Position.Value.Should().Be(3);
    }

    [Fact]
    public void Move_Pet_Should_Move_Other_Pets_When_New_Position_Is_Lower()
    {
        //Arrange            
        Volunteer volunteer = CreateVolunteerWithPets(5);

        Pet firstPet = volunteer.FosteredAnimals[0];
        Pet secondPet = volunteer.FosteredAnimals[1];
        Pet thirdPet = volunteer.FosteredAnimals[2];
        Pet fourthPet = volunteer.FosteredAnimals[3];
        Pet fifthPet = volunteer.FosteredAnimals[4];


        Pet petToMove = fourthPet;
        Position position = Position.Create(2).Value;

        //Act
        var result = volunteer.MovePet(petToMove, position);

        //Assert
         result.IsSuccess.Should().BeTrue();
         result.IsFailure.Should().BeFalse();

         firstPet.Position.Value.Should().Be(1);
         secondPet.Position.Value.Should().Be(3);
         thirdPet.Position.Value.Should().Be(4);
         fourthPet.Position.Value.Should().Be(2);
         fifthPet.Position.Value.Should().Be(5);
    }

    [Fact]
    public void Move_Pet_Should_Move_Other_Pets_When_New_Position_Is_Upper()
    {
        //Arrange            
        Volunteer volunteer = CreateVolunteerWithPets(5);

        Pet firstPet = volunteer.FosteredAnimals[0];
        Pet secondPet = volunteer.FosteredAnimals[1];
        Pet thirdPet = volunteer.FosteredAnimals[2];
        Pet fourthPet = volunteer.FosteredAnimals[3];
        Pet fifthPet = volunteer.FosteredAnimals[4];


        Pet petToMove = secondPet;
        Position position = Position.Create(4).Value;

        //Act
        var result = volunteer.MovePet(petToMove, position);

        //Assert
         result.IsSuccess.Should().BeTrue();
         result.IsFailure.Should().BeFalse();

         firstPet.Position.Value.Should().Be(1);
         secondPet.Position.Value.Should().Be(4);
         thirdPet.Position.Value.Should().Be(2);
         fourthPet.Position.Value.Should().Be(3);
         fifthPet.Position.Value.Should().Be(5);
    }

    [Fact]
    public void Move_Pet_Should_Move_Other_Pets_Back_When_New_Position_Is_First()
    {
        //Arrange            
        Volunteer volunteer = CreateVolunteerWithPets(5);

        Pet firstPet = volunteer.FosteredAnimals[0];
        Pet secondPet = volunteer.FosteredAnimals[1];
        Pet thirdPet = volunteer.FosteredAnimals[2];
        Pet fourthPet = volunteer.FosteredAnimals[3];
        Pet fifthPet = volunteer.FosteredAnimals[4];

        Pet petToMove = fifthPet;

        //Act
        var result = volunteer.MovePetToStart(petToMove);

        //Assert
         result.IsSuccess.Should().BeTrue();
         result.IsFailure.Should().BeFalse();

         firstPet.Position.Value.Should().Be(2);
         secondPet.Position.Value.Should().Be(3);
         thirdPet.Position.Value.Should().Be(4);
         fourthPet.Position.Value.Should().Be(5);
         fifthPet.Position.Value.Should().Be(1);
    }

    [Fact]
    public void Move_Pet_Should_Move_Other_Pets_Forward_When_New_Position_Is_Last()
    {
        //Arrange            
        Volunteer volunteer = CreateVolunteerWithPets(5);

        Pet firstPet = volunteer.FosteredAnimals[0];
        Pet secondPet = volunteer.FosteredAnimals[1];
        Pet thirdPet = volunteer.FosteredAnimals[2];
        Pet fourthPet = volunteer.FosteredAnimals[3];
        Pet fifthPet = volunteer.FosteredAnimals[4];

        Pet petToMove = firstPet;

        //Act
        var result = volunteer.MovePetToEnd(petToMove);

        //Assert
         result.IsSuccess.Should().BeTrue();
         result.IsFailure.Should().BeFalse();

         firstPet.Position.Value.Should().Be(5);
         secondPet.Position.Value.Should().Be(1);
         thirdPet.Position.Value.Should().Be(2);
         fourthPet.Position.Value.Should().Be(3);
         fifthPet.Position.Value.Should().Be(4);
    }

    [Fact]
    public void Move_Pet_Should_Return_Error_When_Pet_Not_Belong_Volunteer()
    {
        //Arrange            
        Volunteer volunteer = CreateVolunteerWithPets(3);

        Pet petToMove = CreatePet();

        //Act
        var result = volunteer.MovePetToEnd(petToMove);

        //Assert
         result.IsSuccess.Should().BeFalse();
         result.IsFailure.Should().BeTrue();
         result.Error.Should().NotBeNull();
    }

    private Volunteer CreateVolunteerWithPets(int petsCount)
    {
        VolunteerId id = VolunteerId.NewVolunteerId();
        FullName fullName = FullName.Create("FirstName", "LastName", null).Value;
        Email email = Email.Create("any@email.com").Value;
        Description description = Description.Create(DESCRIPTION).Value;
        WorkExperience workExpirience = WorkExperience.Create(3).Value;
        PhoneNumber phoneNumber = PhoneNumber.Create(PHONE_NUMBER).Value;
        List<AssistanceRequisites> requisites = [];
        List<SocialNetContact> socialNetContacts = [];

        Volunteer volunteer = new(id, fullName, email, description, workExpirience, phoneNumber,
            requisites, socialNetContacts);

        if (petsCount > 0)
        {
            for (int i = 0; i < petsCount; i++)
            {
                Pet pet = CreatePet();
                 volunteer.AddPet(pet);
            }
        }

        return volunteer;
    }

    private Pet CreatePet()
    {
        PetId id = PetId.NewPetId();
        PetName name = PetName.Create("PetName").Value;
        Description description = Description.Create(DESCRIPTION).Value;
        PetColoration petColoration = PetColoration.Create("Any color").Value;
        PetHealth pethealth = PetHealth.Create("Health status").Value;
        Address address = Address.Create("Region", "City", "Street", "HouseNum", "ApartmentNum").Value;
        Characteristics characteristics = Characteristics.Create(25, 15).Value;
        PhoneNumber phoneNumber = PhoneNumber.Create(PHONE_NUMBER).Value;
        DateTimeOffset birthDay = new(2024, 10, 2, 0, 0, 0, new TimeSpan(0));

        SpecieId specieId = SpecieId.NewSpecieId();
        SpecieName specieName = SpecieName.Create("SpecieName").Value;        

        BreedId breedId = BreedId.NewBreedId();
        BreedName breedName = BreedName.Create("BreedName").Value;        

        return new Pet(id, name, specieId, description, breedId, petColoration,
            pethealth, address, characteristics, phoneNumber, false, birthDay, true,
            AssistanceStatus.LookingForHome, []);
    }
}
