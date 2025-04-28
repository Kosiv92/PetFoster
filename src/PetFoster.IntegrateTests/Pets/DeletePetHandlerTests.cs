using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.DeletePet;
using PetFoster.Domain.Ids;

namespace PetFoster.IntegrateTests.Pets
{
    public class DeletePetHandlerTests : PetTestBase
    {
        private readonly ICommandHandler<Guid, DeletePetCommand> _sut;

        public DeletePetHandlerTests(IntegrationTestsWebFactory factory) 
            : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
                .GetRequiredService<ICommandHandler<Guid, DeletePetCommand>>();
        }

        [Fact]
        public async Task Delete_pet_from_database_return_success()
        {
            //Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerId = Guid.NewGuid();
            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();
            var petId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            var command = new DeletePetCommand(volunteerId, petId);                        

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);
            var volunteer = await VolunteerRepository
                .GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);
            var pet = volunteer.FosteredAnimals.Single();

            //Assert
            result.IsSuccess.Should().BeTrue();            
            result.Value.Should().Be(petId);
            pet.IsDeleted.Should().BeTrue();            
        }

        [Fact]
        public async Task Delete_not_exist_pet_from_database_return_failure()
        {
            //Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerId = Guid.NewGuid();
            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();
            var petId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            var command = new DeletePetCommand(volunteerId, Guid.NewGuid());

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);
            var volunteer = await VolunteerRepository
                .GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);
            var pet = volunteer.FosteredAnimals.Single();

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.Validation);
            pet.Should().NotBeNull();
            pet.IsDeleted.Should().BeFalse();
        }
    }
}
