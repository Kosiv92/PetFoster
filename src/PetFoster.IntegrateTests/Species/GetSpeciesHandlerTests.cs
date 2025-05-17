using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Species.GetSpecies;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Specie;

namespace PetFoster.IntegrateTests.Species
{
    public class GetSpeciesHandlerTests : SpecieTestBase
    {
        private readonly IQueryHandler<PagedList<SpecieDto>, GetSpeciesWithPaginationQuery> _sut;

        public GetSpeciesHandlerTests(IntegrationTestsWebFactory factory) : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
                .GetRequiredService<IQueryHandler<PagedList<SpecieDto>, GetSpeciesWithPaginationQuery>>();
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        public async Task Get_species_return_result(int specieCount)
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            List<Guid> specieIds = [];
            for (int i = 0; i < specieCount; i++)
            {
                specieIds.Add(Guid.NewGuid());
            }

            await SeedDatabase(specieIds, cancellationToken);

            const int pageIndex = 1;

            GetSpeciesWithPaginationQuery query = new(
                null,
                null,
                pageIndex,
                specieCount);

            //Act
            PagedList<SpecieDto> result = await _sut.Handle(query, cancellationToken);

            //Arrange
            _ = result.Should().NotBeNull();
            _ = result.Items.Count.Should().Be(specieCount);
            _ = result.TotalCount.Should().Be(specieCount);
            _ = result.Items.Should().Contain(d => d.Id == specieIds[0]);
        }

        [Theory]
        [InlineData(3, 2, 1)]
        [InlineData(5, 3, 2)]
        public async Task Get_species_second_page_return_part_of_result(
            int specieCount, int pageSize, int partResultCount)
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            List<Guid> specieIds = [];
            for (int i = 0; i < specieCount; i++)
            {
                specieIds.Add(Guid.NewGuid());
            }

            await SeedDatabase(specieIds, cancellationToken);

            const int pageIndex = 2;

            GetSpeciesWithPaginationQuery query = new(
                null,
                null,
                pageIndex,
                pageSize);

            //Act
            PagedList<SpecieDto> result = await _sut.Handle(query, cancellationToken);

            //Arrange
            _ = result.Should().NotBeNull();
            _ = result.TotalCount.Should().Be(specieCount);
            _ = result.Items.Count.Should().Be(partResultCount);
            _ = result.Items.Select(p => p.Id).Should()
                .OnlyContain(id => specieIds.Contains(id));
        }

        [Fact]
        public async Task Get_species_from_not_exist_page_return_empty_result()
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            int specieCount = 2;

            List<Guid> specieIds = [];
            for (int i = 0; i < specieCount; i++)
            {
                specieIds.Add(Guid.NewGuid());
            }

            await SeedDatabase(specieIds, cancellationToken);

            const int pageIndex = 2;

            GetSpeciesWithPaginationQuery query = new(
                null,
                null,
                pageIndex,
                specieCount);

            //Act
            PagedList<SpecieDto> result = await _sut.Handle(query, cancellationToken);

            //Arrange
            _ = result.Should().NotBeNull();
            _ = result.TotalCount.Should().Be(specieCount);
            _ = result.Items.Count.Should().Be(0);
        }
    }
}
