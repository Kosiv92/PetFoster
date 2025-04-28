using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.UploadFilesToPet;
using PetFoster.Domain.Ids;

namespace PetFoster.IntegrateTests.Pets
{
    public class UploadFilesToPetHandlerTests : PetTestBase
    {
        private readonly ICommandHandler<Guid, UploadFilesToPetCommand> _sut;

        public UploadFilesToPetHandlerTests(IntegrationTestsWebFactory factory) 
            : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
                .GetRequiredService<ICommandHandler<Guid, UploadFilesToPetCommand>>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public async Task Update_pet_files_return_success(int fileCount)
        {
            //Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerId = Guid.NewGuid();
            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();
            var petId = Guid.NewGuid();
            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            var command = Fixture.CreateUploadFilesToPetCommand(volunteerId, petId, fileCount);
            Factory.SetupFileProviderMock(command.Files.Select(f => f.FileName));

            //Act
            var result = await _sut.Handle(command, cancellationToken);
            var volunteer = await VolunteerRepository
                .GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);
            var pet = volunteer.FosteredAnimals.Single();

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(petId);
            pet.Id.Value.Should().Be(petId);
            pet.FileList.Should().HaveCount(fileCount);
        }

        [Fact]
        public async Task Update_pet_files_in_not_exist_pet_return_failure()
        {
            //Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerId = Guid.NewGuid();
            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();
            var petId = Guid.NewGuid();
            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            int fileCount = 2;
            var command = Fixture.CreateUploadFilesToPetCommand(volunteerId, Guid.NewGuid(), fileCount);
            Factory.SetupFileProviderMock(command.Files.Select(f => f.FileName));

            //Act
            var result = await _sut.Handle(command, cancellationToken);
            var volunteer = await VolunteerRepository
                .GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);
            var pet = volunteer.FosteredAnimals.Single();

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.Validation);
            pet.Id.Value.Should().Be(petId);
            pet.FileList.Should().HaveCount(0);
        }

        [Fact] 
        public async Task Update_pet_files_with_file_provider_error_return_failure()
        {
            //Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerId = Guid.NewGuid();
            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();
            var petId = Guid.NewGuid();
            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            int fileCount = 2;
            var command = Fixture.CreateUploadFilesToPetCommand(volunteerId, petId, fileCount);

            Factory.SetupFailureFileProviderMock();

            //Act
            var result = await _sut.Handle(command, cancellationToken);
            var volunteer = await VolunteerRepository
                .GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);
            var pet = volunteer.FosteredAnimals.Single();

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.Failure);
            pet.Id.Value.Should().Be(petId);
            pet.FileList.Should().HaveCount(0);
        }
    }
}
