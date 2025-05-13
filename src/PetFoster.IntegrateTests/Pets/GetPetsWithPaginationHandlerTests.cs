using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.GetPets;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Shared;

namespace PetFoster.IntegrateTests.Pets;

public class GetPetsWithPaginationHandlerTests : PetTestBase
{
    private readonly IQueryHandler<PagedList<PetDto>, GetPetsWithPaginationQuery> _sut;

    public GetPetsWithPaginationHandlerTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = ServiceScope.ServiceProvider
                .GetRequiredService<IQueryHandler<PagedList<PetDto>, GetPetsWithPaginationQuery>>();
    }

    [Fact]
    public async Task Get_pet_from_database_return_result()
    {
        //Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var volunteerId = Guid.NewGuid();
        var specieId = Guid.NewGuid();
        var breedId = Guid.NewGuid();
        var petId = Guid.NewGuid();

        await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

        var query = new GetPetsWithPaginationQuery(1, 5, null, default, null);

        //Act
        var result = await _sut.Handle(query, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        result.Items.Single().Id.Should().Be(petId);
    }

    [Fact]
    public async Task Get_pet_with_name_filter_return_expected_result()
    {
        //Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var expectedName = "pet_name_1";

        var volunteerId = Guid.NewGuid();
        var specieId = Guid.NewGuid();
        var breedId = Guid.NewGuid();
        var first_pet = Fixture.CreatePet(Guid.NewGuid(), specieId, breedId, expectedName, null);
        var second_pet = Fixture.CreatePet(Guid.NewGuid(), specieId, breedId, null, null);
        var pets = new List<Pet>() { first_pet, second_pet };

        await SeedDatabaseWithExistPets(volunteerId, specieId, breedId, pets, cancellationToken);

        var filter = new Dictionary<string, (string, string)>();
        filter.Add("name", ("=", expectedName));

        var query = new GetPetsWithPaginationQuery(1, 5, null, default, filter);

        //Act
        var result = await _sut.Handle(query, cancellationToken);

        //Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        result.Items.Single().Name.Should().Be(expectedName);
    }

    [Theory]
    [InlineData("Green", "Blue", true)]
    [InlineData("Blue", "Yellow", false)]
    public async Task Get_pet_with_coloration_sort_return_expected_result(
        string firstColoration, string secondColoration, bool isAscSort)
    {
        //Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var volunteerId = Guid.NewGuid();
        var specieId = Guid.NewGuid();
        var breedId = Guid.NewGuid();
        var first_pet = Fixture.CreatePet(Guid.NewGuid(), specieId, breedId, null, firstColoration);
        var second_pet = Fixture.CreatePet(Guid.NewGuid(), specieId, breedId, null, secondColoration);
        var pets = new List<Pet>() { first_pet, second_pet };

        await SeedDatabaseWithExistPets(volunteerId, specieId, breedId, pets, cancellationToken);

        var sortProperty = "coloration";

        var query = new GetPetsWithPaginationQuery(1, 5, sortProperty, isAscSort, null);

        //Act
        var result = await _sut.Handle(query, cancellationToken);

        //Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items[0].Coloration.Should().Be(secondColoration);
        result.Items[1].Coloration.Should().Be(firstColoration);
    }

    [Theory]
    [InlineData("Ricky", "Alpha", true)]
    [InlineData("Betty", "Cooper", false)]
    public async Task Get_pet_with_filter_and_sort_return_expected_result(
        string firstName, string secondName, bool isAscSort)
    {
        //Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var coloration = "white";

        var volunteerId = Guid.NewGuid();
        var specieId = Guid.NewGuid();
        var breedId = Guid.NewGuid();
        var first_pet = Fixture.CreatePet(Guid.NewGuid(), specieId, breedId, firstName, coloration);
        var second_pet = Fixture.CreatePet(Guid.NewGuid(), specieId, breedId, secondName, coloration);
        var third_pet = Fixture.CreatePet(Guid.NewGuid(), specieId, breedId, null, null);
        var pets = new List<Pet>() { first_pet, second_pet, third_pet };

        await SeedDatabaseWithExistPets(volunteerId, specieId, breedId, pets, cancellationToken);

        var filter = new Dictionary<string, (string, string)>();
        filter.Add("coloration", ("=", coloration));

        var sortProperty = "name";

        var query = new GetPetsWithPaginationQuery(1, 5, sortProperty, isAscSort, filter);

        //Act
        var result = await _sut.Handle(query, cancellationToken);

        //Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
        result.Items[0].Name.Should().Be(second_pet.Name.Value);
        result.Items[1].Name.Should().Be(first_pet.Name.Value);
    }
}

