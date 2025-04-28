using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.UpdatePetStatus;
using PetFoster.Domain.Enums;
using PetFoster.Domain.Ids;

namespace PetFoster.IntegrateTests.Pets
{
    public class UpdatePetStatusHandlerTests : PetTestBase
    {
        private readonly ICommandHandler<Guid, UpdatePetStatusCommand> _sut;

        public UpdatePetStatusHandlerTests(IntegrationTestsWebFactory factory) 
            : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
                .GetRequiredService<ICommandHandler<Guid, UpdatePetStatusCommand>>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task Update_valid_pet_status_return_success(int assistanceStatusCode)
        {
            //Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerId = Guid.NewGuid();
            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();
            var petId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            var assistanceStatus = ((AssistanceStatus)assistanceStatusCode).ToString();

            var command = new UpdatePetStatusCommand(volunteerId, petId, assistanceStatus);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);
            var volunteer = await VolunteerRepository
                .GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);
            var pet = volunteer!.FosteredAnimals.Single();

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(petId);
            pet.Id.Value.Should().Be(petId);            
            pet.AssistanceStatus.Should().Be((AssistanceStatus)assistanceStatusCode);
        }

        [Fact]
        public async Task Update_invalid_pet_status_return_failure()
        {
            //Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerId = Guid.NewGuid();
            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();
            var petId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            var assistanceStatus = AssistanceStatus.FoundHome;

            var command = new UpdatePetStatusCommand(volunteerId, petId, assistanceStatus.ToString());

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);
            var volunteer = await VolunteerRepository
                .GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);
            var pet = volunteer!.FosteredAnimals.Single();

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.Validation);
            pet.Id.Value.Should().Be(petId);
            pet.AssistanceStatus.Should().NotBe(assistanceStatus);
        }

    }
}
