using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Volunteers.GetVolunteers;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;

namespace PetFoster.IntegrateTests.Volunteers
{
    public class GetVolunteersHandlerTests : VolunteerTestBase
    {
        private readonly IQueryHandler<PagedList<VolunteerDto>, GetVoluteersWithPaginationQuery> _sut;

        public GetVolunteersHandlerTests(IntegrationTestsWebFactory factory) : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
                    .GetRequiredService<IQueryHandler<PagedList<VolunteerDto>, GetVoluteersWithPaginationQuery>>();
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        public async Task Get_volunteers_return_result(int volunteerCount)
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            List<Guid> volunteerIds = [];
            for (int i = 0; i < volunteerCount; i++)
            {
                volunteerIds.Add(Guid.NewGuid());
            }

            await SeedDatabase(volunteerIds, cancellationToken);

            const int pageIndex = 1;

            GetVoluteersWithPaginationQuery query = new(
                null,
                null,
                pageIndex,
                volunteerCount);

            //Act
            PagedList<VolunteerDto> result = await _sut.Handle(query, cancellationToken);

            //Arrange
            _ = result.Should().NotBeNull();
            _ = result.TotalCount.Should().Be(volunteerCount);
            _ = result.Items.Should().Contain(d => d.Id == volunteerIds[0]);
        }

        [Theory]
        [InlineData(3, 2, 1)]
        [InlineData(5, 3, 2)]
        public async Task Get_volunteers_from_database_second_page_return_part_of_result(
            int volunteerCount, int pageSize, int partResultCount)
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            List<Guid> volunteerIds = [];
            for (int i = 0; i < volunteerCount; i++)
            {
                volunteerIds.Add(Guid.NewGuid());
            }

            await SeedDatabase(volunteerIds, cancellationToken);

            const int pageIndex = 2;

            GetVoluteersWithPaginationQuery query = new(
                null,
                null,
                pageIndex,
                pageSize);

            //Act
            PagedList<VolunteerDto> result = await _sut.Handle(query, cancellationToken);

            //Arrange
            _ = result.Should().NotBeNull();
            _ = result.TotalCount.Should().Be(volunteerCount);
            _ = result.Items.Count.Should().Be(partResultCount);
            _ = result.Items.Select(p => p.Id).Should()
                .OnlyContain(id => volunteerIds.Contains(id));
        }

        [Fact]
        public async Task Get_volunteers_from_not_exist_page_return_empty_result()
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            int volunteerCount = 2;

            List<Guid> volunteerIds = [];
            for (int i = 0; i < volunteerCount; i++)
            {
                volunteerIds.Add(Guid.NewGuid());
            }

            await SeedDatabase(volunteerIds, cancellationToken);

            const int pageIndex = 2;

            GetVoluteersWithPaginationQuery query = new(
                null,
                null,
                pageIndex,
                volunteerCount);

            //Act
            PagedList<VolunteerDto> result = await _sut.Handle(query, cancellationToken);

            //Arrange
            _ = result.Should().NotBeNull();
            _ = result.TotalCount.Should().Be(volunteerCount);
            _ = result.Items.Count.Should().Be(0);
        }
    }
}
