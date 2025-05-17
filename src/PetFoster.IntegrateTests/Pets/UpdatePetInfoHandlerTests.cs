using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Volunteers.UpdatePetInfo;
using PetFoster.Core.Abstractions;
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
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid volunteerId = Guid.NewGuid();
            Guid specieId = Guid.NewGuid();
            Guid breedId = Guid.NewGuid();
            Guid petId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            UpdatePetInfoCommand command = Fixture.CreateUpdatePetCommand(volunteerId, petId, specieId, breedId);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, CancellationToken.None);
            Domain.Entities.Volunteer? volunteer = await VolunteerRepository
                .GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);
            Domain.Entities.Pet pet = volunteer.FosteredAnimals.Single();

            //Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().Be(petId);
            _ = pet.Id.Value.Should().Be(petId);
            _ = pet.Name.Value.Should().Be(command.Name);
            _ = pet.Description.Value.Should().Be(command.Description);
            _ = pet.Health.Value.Should().Be(command.Health);
            _ = pet.Coloration.Value.Should().Be(command.Coloration);
            _ = pet.IsVaccinated.Should().Be(command.IsVaccinated);
            _ = pet.IsCastrated.Should().Be(command.IsCastrated);
            _ = pet.OwnerPhoneNumber.Value.Should().Be(command.OwnerPhoneNumber);
            _ = pet.SpecieId.Value.Should().Be(command.SpecieId);
            _ = pet.BreedId.Value.Should().Be(command.BreedId);
            _ = pet.Characteristics.Height.Should().Be(command.Characteristics.Height);
            _ = pet.Characteristics.Weight.Should().Be(command.Characteristics.Weight);
            _ = pet.Address.Region.Should().Be(command.Address.Region);
            _ = pet.Address.City.Should().Be(command.Address.City);
            _ = pet.Address.Street.Should().Be(command.Address.Street);
            _ = pet.Address.HouseNumber.Should().Be(command.Address.HouseNumber);
            _ = pet.Address.ApartmentNumber.Should().Be(command.Address.ApartmentNumber);
            _ = pet.AssistanceStatus.ToString().Should().Be(command.AssistanceStatus);
        }

        [Fact]
        public async Task Update_not_exist_pet_return_failure()
        {
            //Arrange
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid volunteerId = Guid.NewGuid();
            Guid specieId = Guid.NewGuid();
            Guid breedId = Guid.NewGuid();
            Guid petId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            UpdatePetInfoCommand command = Fixture.CreateUpdatePetCommand(volunteerId, Guid.NewGuid(), specieId, breedId);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, CancellationToken.None);
            Domain.Entities.Volunteer? volunteer = await VolunteerRepository
                .GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);
            Domain.Entities.Pet pet = volunteer.FosteredAnimals.Single();

            //Assert
            _ = result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.Validation);
            _ = pet.Name.Value.Should().NotBe(command.Name);
            _ = pet.Description.Value.Should().NotBe(command.Description);
            _ = pet.Health.Value.Should().NotBe(command.Health);
            _ = pet.Coloration.Value.Should().NotBe(command.Coloration);
            _ = pet.OwnerPhoneNumber.Value.Should().NotBe(command.OwnerPhoneNumber);
        }
    }
}
