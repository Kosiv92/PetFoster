using FluentAssertions;
using PetFoster.Core.ValueObjects;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;

namespace PetFoster.UnitTests.Domain
{
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
            _ = pet.Id;

            //Act
            CSharpFunctionalExtensions.UnitResult<Core.Error> result = volunteer.AddPet(pet);

            //Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.IsFailure.Should().BeFalse();
            _ = pet.Position.Should().Be(Position.First);
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
            _ = pet.Id;

            //Act
            CSharpFunctionalExtensions.UnitResult<Core.Error> result = volunteer.AddPet(pet);

            //Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.IsFailure.Should().BeFalse();
            _ = pet.Position.Should()
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
            CSharpFunctionalExtensions.UnitResult<Core.Error> result = volunteer.MovePet(petToMove, position);

            //Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.IsFailure.Should().BeFalse();

            _ = firstPet.Position.Value.Should().Be(1);
            _ = secondPet.Position.Value.Should().Be(2);
            _ = thirdPet.Position.Value.Should().Be(3);
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
            CSharpFunctionalExtensions.UnitResult<Core.Error> result = volunteer.MovePet(petToMove, position);

            //Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.IsFailure.Should().BeFalse();

            _ = firstPet.Position.Value.Should().Be(1);
            _ = secondPet.Position.Value.Should().Be(3);
            _ = thirdPet.Position.Value.Should().Be(4);
            _ = fourthPet.Position.Value.Should().Be(2);
            _ = fifthPet.Position.Value.Should().Be(5);
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
            CSharpFunctionalExtensions.UnitResult<Core.Error> result = volunteer.MovePet(petToMove, position);

            //Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.IsFailure.Should().BeFalse();

            _ = firstPet.Position.Value.Should().Be(1);
            _ = secondPet.Position.Value.Should().Be(4);
            _ = thirdPet.Position.Value.Should().Be(2);
            _ = fourthPet.Position.Value.Should().Be(3);
            _ = fifthPet.Position.Value.Should().Be(5);
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
            CSharpFunctionalExtensions.UnitResult<Core.Error> result = volunteer.MovePetToStart(petToMove);

            //Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.IsFailure.Should().BeFalse();

            _ = firstPet.Position.Value.Should().Be(2);
            _ = secondPet.Position.Value.Should().Be(3);
            _ = thirdPet.Position.Value.Should().Be(4);
            _ = fourthPet.Position.Value.Should().Be(5);
            _ = fifthPet.Position.Value.Should().Be(1);
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
            CSharpFunctionalExtensions.UnitResult<Core.Error> result = volunteer.MovePetToEnd(petToMove);

            //Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.IsFailure.Should().BeFalse();

            _ = firstPet.Position.Value.Should().Be(5);
            _ = secondPet.Position.Value.Should().Be(1);
            _ = thirdPet.Position.Value.Should().Be(2);
            _ = fourthPet.Position.Value.Should().Be(3);
            _ = fifthPet.Position.Value.Should().Be(4);
        }

        [Fact]
        public void Move_Pet_Should_Return_Error_When_Pet_Not_Belong_Volunteer()
        {
            //Arrange            
            Volunteer volunteer = CreateVolunteerWithPets(3);

            Pet petToMove = CreatePet();

            //Act
            CSharpFunctionalExtensions.UnitResult<Core.Error> result = volunteer.MovePetToEnd(petToMove);

            //Assert
            _ = result.IsSuccess.Should().BeFalse();
            _ = result.IsFailure.Should().BeTrue();
            _ = result.Error.Should().NotBeNull();
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
                    _ = volunteer.AddPet(pet);
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
            _ = new
            Specie(specieId, specieName);

            BreedId breedId = BreedId.NewBreedId();
            BreedName breedName = BreedName.Create("BreedName").Value;
            _ = new
            Breed(breedId, breedName);

            return new Pet(id, name, specieId, description, breedId, petColoration,
                pethealth, address, characteristics, phoneNumber, false, birthDay, true,
                PetFoster.Domain.Enums.AssistanceStatus.LookingForHome, []);
        }
    }
}
