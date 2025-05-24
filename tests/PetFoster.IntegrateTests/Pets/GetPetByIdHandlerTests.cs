using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Volunteers.Application.PetManagement.GetPetById;

namespace PetFoster.IntegrateTests.Pets;

public class GetPetByIdHandlerTests : PetTestBase
{
    private readonly IQueryHandler<PetDto, GetPetByIdQuery> _sut;

    public GetPetByIdHandlerTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = ServiceScope.ServiceProvider
                .GetRequiredService<IQueryHandler<PetDto, GetPetByIdQuery>>();
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

        GetPetByIdQuery query = new(volunteerId, petId);

        //Act
        PetDto result = await _sut.Handle(query, CancellationToken.None);


        //Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(petId);
    }

    [Fact]
    public async Task Get_not_exist_pet_from_database_return_null()
    {
        //Arrange
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        Guid volunteerId = Guid.NewGuid();
        Guid specieId = Guid.NewGuid();
        Guid breedId = Guid.NewGuid();
        Guid petId = Guid.NewGuid();

        await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

        GetPetByIdQuery query = new(volunteerId, Guid.NewGuid());

        //Act
        PetDto result = await _sut.Handle(query, CancellationToken.None);


        //Assert
        result.Should().BeNull();
    }
}
