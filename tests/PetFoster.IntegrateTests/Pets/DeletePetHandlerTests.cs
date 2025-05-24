using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Core.Abstractions;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.SharedKernel;
using PetFoster.Volunteers.Application.PetManagement.DeletePet;
using PetFoster.Volunteers.Domain.Entities;

namespace PetFoster.IntegrateTests.Pets;

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
        var result = await _sut.Handle(command, CancellationToken.None);
        Volunteer? volunteer = await VolunteerRepository
            .GetByIdAsync(
            VolunteerId.Create(volunteerId),
            cancellationToken);
        Pet pet = volunteer.FosteredAnimals.Single();

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(petId);
        pet.IsDeleted.Should().BeTrue();
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
        var result = await _sut.Handle(command, CancellationToken.None);
        Volunteer? volunteer = await VolunteerRepository
            .GetByIdAsync(
            VolunteerId.Create(volunteerId),
            cancellationToken);
        Pet pet = volunteer.FosteredAnimals.Single();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Single().Type.Should().Be(ErrorType.Validation);
        pet.Should().NotBeNull();
        pet.IsDeleted.Should().BeFalse();
    }
}
