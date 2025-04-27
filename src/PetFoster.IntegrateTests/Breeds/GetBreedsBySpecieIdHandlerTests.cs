using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.DTO.Specie;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Species.GetBreeds;

namespace PetFoster.IntegrateTests.Breeds
{
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
            var cancellationToken = new CancellationTokenSource().Token;

            int breedsCount = 3;

            var specieId = Guid.NewGuid();
            List<Guid> breedIds = new List<Guid>();
            for(int i = 0; i < breedsCount; i++)
            {
                breedIds.Add(Guid.NewGuid());
            }

            await SeedDatabase(specieId, breedIds, cancellationToken);

            var query = new GetBreedsBySpecieIdQuery(specieId);

            //Act
            var result = await _sut.Handle(query, cancellationToken);

            //Arrange
            result.Should().NotBeNull();
            result.Count.Should().Be(breedsCount);            
        }

        [Fact]
        public async Task Get_breeds_from_not_exist_specie_return_failure()
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;

            int breedsCount = 3;

            var specieId = Guid.NewGuid();
            List<Guid> breedIds = new List<Guid>();
            for (int i = 0; i < breedsCount; i++)
            {
                breedIds.Add(Guid.NewGuid());
            }

            await SeedDatabase(specieId, breedIds, cancellationToken);

            var query = new GetBreedsBySpecieIdQuery(Guid.NewGuid());

            //Act
            var result = await _sut.Handle(query, cancellationToken);

            //Arrange
            result.Should().BeEmpty();            
        }
    }
}
