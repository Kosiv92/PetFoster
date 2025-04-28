using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.AddPet;
using PetFoster.Domain.Ids;

namespace PetFoster.IntegrateTests.Pets
{
    public class AddPetHandlerTests : PetTestBase
    {
        private readonly ICommandHandler<Guid, AddPetCommand> _sut;

        public AddPetHandlerTests(IntegrationTestsWebFactory factory)
            : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
                .GetRequiredService<ICommandHandler<Guid, AddPetCommand>>();
        }

        [Fact]
        public async Task Add_pet_to_database_return_success()
        {
            //Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerId = Guid.NewGuid();
            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, null, cancellationToken);

            var command = Fixture.CreateAddPetCommandComand(volunteerId, specieId, breedId);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);
            var volunteer = await VolunteerRepository
                .GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);
            var pet = volunteer.FosteredAnimals.Single();

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();
            pet.Should().NotBeNull();
            pet.Id.Value.Should().Be(result.Value);
        }

        [Fact]
        public async Task Add_pet_to_not_exist_volunteer_return_failure()
        {
            //Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerId = Guid.NewGuid();
            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, null, cancellationToken);

            var command = Fixture.CreateAddPetCommandComand(Guid.NewGuid(), specieId, breedId);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task Add_pet_with_not_exist_specie_return_failure()
        {
            //Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerId = Guid.NewGuid();
            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, null, cancellationToken);

            var command = Fixture.CreateAddPetCommandComand(volunteerId, Guid.NewGuid(), breedId);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task Add_pet_with_not_exist_breed_return_failure()
        {
            //Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerId = Guid.NewGuid();
            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, null, cancellationToken);

            var command = Fixture.CreateAddPetCommandComand(volunteerId, specieId, Guid.NewGuid());

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.Validation);
        }
    }
}
