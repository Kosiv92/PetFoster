using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Specie;
using PetFoster.Species.Application.BreedManagement.GetBreeds;

namespace PetFoster.IntegrateTests.Breeds;

public class GetBreedsBySpecieIdHandlerTests : BreedTestBase
{
    private readonly IQueryHandler<List<BreedDto>, GetBreedsBySpecieIdQuery> _sut;

    public GetBreedsBySpecieIdHandlerTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = ServiceScope.ServiceProvider
            .GetRequiredService<IQueryHandler<List<BreedDto>, GetBreedsBySpecieIdQuery>>();
    }

    [Fact]
    public async Task Get_breeds_return_result()
    {
        //Assert
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        int breedsCount = 3;

        Guid specieId = Guid.NewGuid();
        List<Guid> breedIds = [];
        for (int i = 0; i < breedsCount; i++)
        {
            breedIds.Add(Guid.NewGuid());
        }

        await SeedDatabase(specieId, breedIds, cancellationToken);

        GetBreedsBySpecieIdQuery query = new(specieId);

        //Act
        List<BreedDto> result = await _sut.Handle(query, cancellationToken);

        //Arrange
        result.Should().NotBeNull();
        result.Count.Should().Be(breedsCount);
    }

    [Fact]
    public async Task Get_breeds_from_not_exist_specie_return_failure()
    {
        //Assert
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        int breedsCount = 3;

        Guid specieId = Guid.NewGuid();
        List<Guid> breedIds = [];
        for (int i = 0; i < breedsCount; i++)
        {
            breedIds.Add(Guid.NewGuid());
        }

        await SeedDatabase(specieId, breedIds, cancellationToken);

        GetBreedsBySpecieIdQuery query = new(Guid.NewGuid());

        //Act
        List<BreedDto> result = await _sut.Handle(query, cancellationToken);

        //Arrange
        result.Should().BeEmpty();
    }
}
