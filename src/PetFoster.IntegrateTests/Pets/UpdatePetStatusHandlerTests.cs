using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Volunteers.UpdatePetStatus;
using PetFoster.Core.Abstractions;
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
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid volunteerId = Guid.NewGuid();
            Guid specieId = Guid.NewGuid();
            Guid breedId = Guid.NewGuid();
            Guid petId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            string assistanceStatus = ((AssistanceStatus)assistanceStatusCode).ToString();

            UpdatePetStatusCommand command = new(volunteerId, petId, assistanceStatus);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, CancellationToken.None);
            Domain.Entities.Volunteer? volunteer = await VolunteerRepository
                .GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);
            Domain.Entities.Pet pet = volunteer!.FosteredAnimals.Single();

            //Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().Be(petId);
            _ = pet.Id.Value.Should().Be(petId);
            _ = pet.AssistanceStatus.Should().Be((AssistanceStatus)assistanceStatusCode);
        }

        [Fact]
        public async Task Update_invalid_pet_status_return_failure()
        {
            //Arrange
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid volunteerId = Guid.NewGuid();
            Guid specieId = Guid.NewGuid();
            Guid breedId = Guid.NewGuid();
            Guid petId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            AssistanceStatus assistanceStatus = AssistanceStatus.FoundHome;

            UpdatePetStatusCommand command = new(volunteerId, petId, assistanceStatus.ToString());

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, CancellationToken.None);
            Domain.Entities.Volunteer? volunteer = await VolunteerRepository
                .GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);
            Domain.Entities.Pet pet = volunteer!.FosteredAnimals.Single();

            //Assert
            _ = result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.Validation);
            _ = pet.Id.Value.Should().Be(petId);
            _ = pet.AssistanceStatus.Should().NotBe(assistanceStatus);
        }

    }
}
