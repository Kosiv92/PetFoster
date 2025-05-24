using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.DTO;
using PetFoster.Core.Models;
using PetFoster.Volunteers.Application.PetManagement.GetPets;
using PetFoster.Volunteers.Domain.Entities;

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
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        Guid volunteerId = Guid.NewGuid();
        Guid specieId = Guid.NewGuid();
        Guid breedId = Guid.NewGuid();
        Guid petId = Guid.NewGuid();

        await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

        GetPetsWithPaginationQuery query = new(1, 5, null, default, null);

        //Act
        PagedList<PetDto> result = await _sut.Handle(query, CancellationToken.None);

        //Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        result.Items.Single().Id.Should().Be(petId);
    }

    [Fact]
    public async Task Get_pet_with_name_filter_return_expected_result()
    {
        //Arrange
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        string expectedName = "pet_name_1";

        Guid volunteerId = Guid.NewGuid();
        Guid specieId = Guid.NewGuid();
        Guid breedId = Guid.NewGuid();
        Pet first_pet = Fixture.CreatePet(Guid.NewGuid(), specieId, breedId, expectedName, null);
        Pet second_pet = Fixture.CreatePet(Guid.NewGuid(), specieId, breedId, null, null);
        List<Pet> pets = [first_pet, second_pet];

        await SeedDatabaseWithExistPets(volunteerId, specieId, breedId, pets, cancellationToken);

        List<FilterItemDto> filter =
        [
            new FilterItemDto("name", "=", expectedName)
        ];

        GetPetsWithPaginationQuery query = new(1, 5, null, default, filter);

        //Act
        PagedList<PetDto> result = await _sut.Handle(query, cancellationToken);

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
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        Guid volunteerId = Guid.NewGuid();
        Guid specieId = Guid.NewGuid();
        Guid breedId = Guid.NewGuid();
        Pet first_pet = Fixture.CreatePet(Guid.NewGuid(), specieId, breedId, null, firstColoration);
        Pet second_pet = Fixture.CreatePet(Guid.NewGuid(), specieId, breedId, null, secondColoration);
        List<Pet> pets = [first_pet, second_pet];

        await SeedDatabaseWithExistPets(volunteerId, specieId, breedId, pets, cancellationToken);

        string sortProperty = "coloration";

        GetPetsWithPaginationQuery query = new(1, 5, sortProperty, isAscSort, null);

        //Act
        PagedList<PetDto> result = await _sut.Handle(query, cancellationToken);

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
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        string coloration = "white";

        Guid volunteerId = Guid.NewGuid();
        Guid specieId = Guid.NewGuid();
        Guid breedId = Guid.NewGuid();
        Pet first_pet = Fixture.CreatePet(Guid.NewGuid(), specieId, breedId, firstName, coloration);
        Pet second_pet = Fixture.CreatePet(Guid.NewGuid(), specieId, breedId, secondName, coloration);
        Pet third_pet = Fixture.CreatePet(Guid.NewGuid(), specieId, breedId, null, null);
        List<Pet> pets = [first_pet, second_pet, third_pet];

        await SeedDatabaseWithExistPets(volunteerId, specieId, breedId, pets, cancellationToken);

        List<FilterItemDto> filter =
        [
            new FilterItemDto("coloration", "=", coloration)
        ];

        string sortProperty = "name";

        GetPetsWithPaginationQuery query = new(1, 5, sortProperty, isAscSort, filter);

        //Act
        PagedList<PetDto> result = await _sut.Handle(query, cancellationToken);

        //Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
        result.Items[0].Name.Should().Be(second_pet.Name.Value);
        result.Items[1].Name.Should().Be(first_pet.Name.Value);
    }
}

