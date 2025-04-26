using FluentAssertions;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.ValueObjects;

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
            var volunteer = CreateVolunteerWithPets(0);

            var pet = CreatePet();

            var petId = pet.Id;

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
            var volunteer = CreateVolunteerWithPets(petsCount);

            var pet = CreatePet();

            var petId = pet.Id;

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
            var volunteer = CreateVolunteerWithPets(3);

            var firstPet = volunteer.FosteredAnimals[0];
            var secondPet = volunteer.FosteredAnimals[1];
            var thirdPet = volunteer.FosteredAnimals[2];

            var petToMove = volunteer.FosteredAnimals[1];
            var position = Position.Create(2).Value;

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
            var volunteer = CreateVolunteerWithPets(5);

            var firstPet = volunteer.FosteredAnimals[0];
            var secondPet = volunteer.FosteredAnimals[1];
            var thirdPet = volunteer.FosteredAnimals[2];
            var fourthPet = volunteer.FosteredAnimals[3];
            var fifthPet = volunteer.FosteredAnimals[4];
            

            var petToMove = fourthPet;
            var position = Position.Create(2).Value;

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
            var volunteer = CreateVolunteerWithPets(5);

            var firstPet = volunteer.FosteredAnimals[0];
            var secondPet = volunteer.FosteredAnimals[1];
            var thirdPet = volunteer.FosteredAnimals[2];
            var fourthPet = volunteer.FosteredAnimals[3];
            var fifthPet = volunteer.FosteredAnimals[4];


            var petToMove = secondPet;
            var position = Position.Create(4).Value;

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
            var volunteer = CreateVolunteerWithPets(5);

            var firstPet = volunteer.FosteredAnimals[0];
            var secondPet = volunteer.FosteredAnimals[1];
            var thirdPet = volunteer.FosteredAnimals[2];
            var fourthPet = volunteer.FosteredAnimals[3];
            var fifthPet = volunteer.FosteredAnimals[4];

            var petToMove = fifthPet;            

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
            var volunteer = CreateVolunteerWithPets(5);

            var firstPet = volunteer.FosteredAnimals[0];
            var secondPet = volunteer.FosteredAnimals[1];
            var thirdPet = volunteer.FosteredAnimals[2];
            var fourthPet = volunteer.FosteredAnimals[3];
            var fifthPet = volunteer.FosteredAnimals[4];

            var petToMove = firstPet;            

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
            var volunteer = CreateVolunteerWithPets(3);                       

            var petToMove = CreatePet();

            //Act
            var result = volunteer.MovePetToEnd(petToMove);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();           
        }

        private Volunteer CreateVolunteerWithPets(int petsCount)
        {
            var id = VolunteerId.NewVolunteerId();
            var fullName = FullName.Create("FirstName", "LastName", null).Value;
            var email = Email.Create("any@email.com").Value;
            var description = Description.Create(DESCRIPTION).Value;
            var workExpirience = WorkExperience.Create(3).Value;
            var phoneNumber = PhoneNumber.Create(PHONE_NUMBER).Value;
            var requisites = new List<AssistanceRequisites>();
            var socialNetContacts = new List<SocialNetContact>();            

            var volunteer = new Volunteer(id, fullName, email, description, workExpirience, phoneNumber,
                requisites, socialNetContacts);

            if (petsCount > 0)
            {
                for (int i = 0; i < petsCount; i++)
                {
                    var pet = CreatePet();
                    volunteer.AddPet(pet);
                }
            }

            return volunteer;
        }

        private Pet CreatePet()
        {
            var id = PetId.NewPetId();
            var name = PetName.Create("PetName").Value;
            var description = Description.Create(DESCRIPTION).Value;
            var petColoration = PetColoration.Create("Any color").Value;
            var pethealth = PetHealth.Create("Health status").Value;
            var address = Address.Create("Region", "City", "Street", "HouseNum", "ApartmentNum").Value;
            var characteristics = Characteristics.Create(25, 15).Value;
            var phoneNumber = PhoneNumber.Create(PHONE_NUMBER).Value;
            var birthDay = new DateTimeOffset(2024, 10, 2, 0, 0, 0, new TimeSpan(0));

            var specieId = SpecieId.NewSpecieId();
            var specieName = SpecieName.Create("SpecieName").Value;

            var specie = new Specie(specieId, specieName);

            var breedId = BreedId.NewBreedId();
            var breedName = BreedName.Create("BreedName").Value;

            var breed = new Breed(breedId, breedName);

            return new Pet(id, name, specieId, description, breedId, petColoration,
                pethealth, address, characteristics, phoneNumber, false, birthDay, true,
                PetFoster.Domain.Enums.AssistanceStatus.LookingForHome, new List<AssistanceRequisites>());
        }
    }
}
