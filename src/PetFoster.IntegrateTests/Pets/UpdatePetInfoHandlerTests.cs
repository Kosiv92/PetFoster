using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.UpdatePetInfo;
using PetFoster.Domain.Ids;

namespace PetFoster.IntegrateTests.Pets
{
    public class UpdatePetInfoHandlerTests : PetTestBase
    {
        private readonly ICommandHandler<Guid, UpdatePetInfoCommand> _sut;

        public UpdatePetInfoHandlerTests(IntegrationTestsWebFactory factory)
            : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
                .GetRequiredService<ICommandHandler<Guid, UpdatePetInfoCommand>>();
        }

        [Fact]
        public async Task Update_pet_return_success()
        {
            //Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerId = Guid.NewGuid();
            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();
            var petId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            var command = Fixture.CreateUpdatePetCommand(volunteerId, petId, specieId, breedId);

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
            pet.Id.Value.Should().Be(petId);
            pet.Name.Value.Should().Be(command.Name);
            pet.Description.Value.Should().Be(command.Description);
            pet.Health.Value.Should().Be(command.Health);
            pet.Coloration.Value.Should().Be(command.Coloration);
            pet.IsVaccinated.Should().Be(command.IsVaccinated);
            pet.IsCastrated.Should().Be(command.IsCastrated);
            pet.OwnerPhoneNumber.Value.Should().Be(command.OwnerPhoneNumber);
            pet.SpecieId.Value.Should().Be(command.SpecieId);
            pet.BreedId.Value.Should().Be(command.BreedId);
            pet.Characteristics.Height.Should().Be(command.Characteristics.Height);
            pet.Characteristics.Weight.Should().Be(command.Characteristics.Weight);
            pet.Address.Region.Should().Be(command.Address.Region);
            pet.Address.City.Should().Be(command.Address.City);
            pet.Address.Street.Should().Be(command.Address.Street);
            pet.Address.HouseNumber.Should().Be(command.Address.HouseNumber);
            pet.Address.ApartmentNumber.Should().Be(command.Address.ApartmentNumber);
            pet.AssistanceStatus.ToString().Should().Be(command.AssistanceStatus);
        }

        [Fact]
        public async Task Update_not_exist_pet_return_failure()
        {
            //Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerId = Guid.NewGuid();
            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();
            var petId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            var command = Fixture.CreateUpdatePetCommand(volunteerId, Guid.NewGuid(), specieId, breedId);

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
            pet.Name.Value.Should().NotBe(command.Name);
            pet.Description.Value.Should().NotBe(command.Description);
            pet.Health.Value.Should().NotBe(command.Health);
            pet.Coloration.Value.Should().NotBe(command.Coloration);            
            pet.OwnerPhoneNumber.Value.Should().NotBe(command.OwnerPhoneNumber);             
        }
    }
}
