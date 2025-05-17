using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Volunteers.DeletePet;
using PetFoster.Core.Abstractions;
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
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid volunteerId = Guid.NewGuid();
            Guid specieId = Guid.NewGuid();
            Guid breedId = Guid.NewGuid();
            Guid petId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            DeletePetCommand command = new(volunteerId, petId);

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
            _ = pet.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_not_exist_pet_from_database_return_failure()
        {
            //Arrange
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid volunteerId = Guid.NewGuid();
            Guid specieId = Guid.NewGuid();
            Guid breedId = Guid.NewGuid();
            Guid petId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            DeletePetCommand command = new(volunteerId, Guid.NewGuid());

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
            _ = pet.Should().NotBeNull();
            _ = pet.IsDeleted.Should().BeFalse();
        }
    }
}
