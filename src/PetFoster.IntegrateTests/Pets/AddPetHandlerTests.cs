using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Volunteers.AddPet;
using PetFoster.Core.Abstractions;
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
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid volunteerId = Guid.NewGuid();
            Guid specieId = Guid.NewGuid();
            Guid breedId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, null, cancellationToken);

            AddPetCommand command = Fixture.CreateAddPetCommandComand(volunteerId, specieId, breedId);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, CancellationToken.None);
            Domain.Entities.Volunteer? volunteer = await VolunteerRepository
                .GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);
            Domain.Entities.Pet pet = volunteer.FosteredAnimals.Single();

            //Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().NotBeEmpty();
            _ = pet.Should().NotBeNull();
            _ = pet.Id.Value.Should().Be(result.Value);
        }

        [Fact]
        public async Task Add_pet_to_not_exist_volunteer_return_failure()
        {
            //Arrange
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid volunteerId = Guid.NewGuid();
            Guid specieId = Guid.NewGuid();
            Guid breedId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, null, cancellationToken);

            AddPetCommand command = Fixture.CreateAddPetCommandComand(Guid.NewGuid(), specieId, breedId);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _ = result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task Add_pet_with_not_exist_specie_return_failure()
        {
            //Arrange
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid volunteerId = Guid.NewGuid();
            Guid specieId = Guid.NewGuid();
            Guid breedId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, null, cancellationToken);

            AddPetCommand command = Fixture.CreateAddPetCommandComand(volunteerId, Guid.NewGuid(), breedId);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _ = result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task Add_pet_with_not_exist_breed_return_failure()
        {
            //Arrange
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid volunteerId = Guid.NewGuid();
            Guid specieId = Guid.NewGuid();
            Guid breedId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, null, cancellationToken);

            AddPetCommand command = Fixture.CreateAddPetCommandComand(volunteerId, specieId, Guid.NewGuid());

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _ = result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.Validation);
        }
    }
}
