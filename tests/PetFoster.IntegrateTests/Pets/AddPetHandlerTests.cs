using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Core.Abstractions;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.SharedKernel;
using PetFoster.Volunteers.Application.PetManagement.AddPet;
using PetFoster.Volunteers.Domain.Entities;

namespace PetFoster.IntegrateTests.Pets;

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

        AddPetCommand command = Fixture
            .CreateAddPetCommandComand(volunteerId, specieId, breedId);

        //Act
        var result = await _sut.Handle(command, CancellationToken.None);
        Volunteer? volunteer = await VolunteerRepository
            .GetByIdAsync(
            VolunteerId.Create(volunteerId),
            cancellationToken);
        Pet pet = volunteer.FosteredAnimals.Single();

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
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        Guid volunteerId = Guid.NewGuid();
        Guid specieId = Guid.NewGuid();
        Guid breedId = Guid.NewGuid();

        await SeedDatabase(volunteerId, specieId, breedId, null, cancellationToken);

        AddPetCommand command = Fixture
            .CreateAddPetCommandComand(Guid.NewGuid(), specieId, breedId);

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
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        Guid volunteerId = Guid.NewGuid();
        Guid specieId = Guid.NewGuid();
        Guid breedId = Guid.NewGuid();

        await SeedDatabase(volunteerId, specieId, breedId, null, cancellationToken);

        AddPetCommand command = Fixture
            .CreateAddPetCommandComand(volunteerId, Guid.NewGuid(), breedId);

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
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        Guid volunteerId = Guid.NewGuid();
        Guid specieId = Guid.NewGuid();
        Guid breedId = Guid.NewGuid();

        await SeedDatabase(volunteerId, specieId, breedId, null, cancellationToken);

        AddPetCommand command = Fixture
            .CreateAddPetCommandComand(volunteerId, specieId, Guid.NewGuid());

        //Act
        var result = await _sut.Handle(command, CancellationToken.None);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Single().Type.Should().Be(ErrorType.Validation);
    }
}
