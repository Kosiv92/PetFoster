using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.GetVolunteers;
using PetFoster.Domain.Shared;

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
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerIds = new List<Guid>();
            for (int i = 0; i < volunteerCount; i++)
            {
                volunteerIds.Add(Guid.NewGuid());
            }

            await SeedDatabase(volunteerIds, cancellationToken);

            const int pageIndex = 1;

            var query = new GetVoluteersWithPaginationQuery(
                null,
                null,
                pageIndex,
                volunteerCount);

            //Act
            var result = await _sut.Handle(query, cancellationToken);

            //Arrange
            result.Should().NotBeNull();
            result.TotalCount.Should().Be(volunteerCount);
            result.Items.Should().Contain(d => d.Id == volunteerIds[0]);
        }

        [Theory]
        [InlineData(3, 2, 1)]
        [InlineData(5, 3, 2)]
        public async Task Get_volunteers_from_database_second_page_return_part_of_result(
            int volunteerCount, int pageSize, int partResultCount)
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerIds = new List<Guid>();
            for (int i = 0; i < volunteerCount; i++)
            {
                volunteerIds.Add(Guid.NewGuid());
            }

            await SeedDatabase(volunteerIds, cancellationToken);

            const int pageIndex = 2;

            var query = new GetVoluteersWithPaginationQuery(
                null,
                null,
                pageIndex,
                pageSize);

            //Act
            var result = await _sut.Handle(query, cancellationToken);

            //Arrange
            result.Should().NotBeNull();
            result.TotalCount.Should().Be(volunteerCount);
            result.Items.Count.Should().Be(partResultCount);
            result.Items.Select(p => p.Id).Should()
                .OnlyContain(id => volunteerIds.Contains(id));            
        }

        [Fact]        
        public async Task Get_volunteers_from_not_exist_page_return_empty_result()
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;

            int volunteerCount = 2;

            var volunteerIds = new List<Guid>();
            for (int i = 0; i < volunteerCount; i++)
            {
                volunteerIds.Add(Guid.NewGuid());
            }

            await SeedDatabase(volunteerIds, cancellationToken);

            const int pageIndex = 2;

            var query = new GetVoluteersWithPaginationQuery(
                null,
                null,
                pageIndex,
                volunteerCount);

            //Act
            var result = await _sut.Handle(query, cancellationToken);

            //Arrange
            result.Should().NotBeNull();
            result.TotalCount.Should().Be(volunteerCount);
            result.Items.Count.Should().Be(0);            
        }
    }
}
