using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Volunteers.GetVolunteer;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;

namespace PetFoster.IntegrateTests.Volunteers
{
    public sealed class GetVolunteerByIdHandlerTests : VolunteerTestBase
    {
        private readonly IQueryHandler<VolunteerDto, GetVolunteerByIdQuery> _sut;

        public GetVolunteerByIdHandlerTests(IntegrationTestsWebFactory factory)
            : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
                    .GetRequiredService<IQueryHandler<VolunteerDto, GetVolunteerByIdQuery>>();
        }

        [Fact]
        public async Task Get_volunteer_by_id_from_database_return_result()
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            int volunteerCount = 3;

            List<Guid> volunteerIds = [];
            for (int i = 0; i < volunteerCount; i++)
            {
                volunteerIds.Add(Guid.NewGuid());
            }

            await SeedDatabase(volunteerIds, cancellationToken);

            Guid idToGet = volunteerIds.Last();

            GetVolunteerByIdQuery query = new(idToGet);


            //Act
            VolunteerDto result = await _sut.Handle(query, cancellationToken);

            //Arrange
            _ = result.Should().NotBeNull();
            _ = result.Id.Should().Be(idToGet);
        }

        [Fact]
        public async Task Get_volunteer_by_wrong_id_from_database_return_null()
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            int volunteerCount = 3;

            List<Guid> volunteerIds = [];
            for (int i = 0; i < volunteerCount; i++)
            {
                volunteerIds.Add(Guid.NewGuid());
            }

            await SeedDatabase(volunteerIds, cancellationToken);

            Guid idToGet = Guid.NewGuid();

            GetVolunteerByIdQuery query = new(idToGet);


            //Act
            VolunteerDto result = await _sut.Handle(query, cancellationToken);

            //Arrange
            _ = result.Should().BeNull();
        }

    }
}
