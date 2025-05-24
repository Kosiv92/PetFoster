using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Core.Abstractions;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.SharedKernel;
using PetFoster.Species.Application.BreedManagement.AddBreed;
using PetFoster.Species.Domain.Entities;

namespace PetFoster.IntegrateTests.Breeds;

public class AddBreedHandlerTests : BreedTestBase
{
    private readonly ICommandHandler<Guid, AddBreedCommand> _sut;

    public AddBreedHandlerTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = ServiceScope.ServiceProvider
            .GetRequiredService<ICommandHandler<Guid, AddBreedCommand>>();
    }

    [Fact]
    public async Task Add_breed_to_database_return_success()
    {
        //Arrange
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        Guid specieId = Guid.NewGuid();

        await SeedDatabase([specieId], cancellationToken);

        AddBreedCommand command = new(specieId, Fixture.Create<string>());

        //Act
        var result = await _sut.Handle(command, CancellationToken.None);
        Specie? specie = await SpecieRepository
            .GetByIdAsync(
            SpecieId.Create(specieId),
            cancellationToken);
        Breed breed = specie.Breeds.Single();

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        breed.Should().NotBeNull();
        breed.Id.Value.Should().Be(result.Value);
    }

    [Fact]
    public async Task Add_pet_to_not_exist_specie_return_failure()
    {
        //Arrange
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        Guid specieId = Guid.NewGuid();

        await SeedDatabase([specieId], cancellationToken);

        AddBreedCommand command = new(Guid.NewGuid(), Fixture.Create<string>());

        //Act
        var result = await _sut.Handle(command, CancellationToken.None);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Single().Type.Should().Be(ErrorType.NotFound);
    }
}
