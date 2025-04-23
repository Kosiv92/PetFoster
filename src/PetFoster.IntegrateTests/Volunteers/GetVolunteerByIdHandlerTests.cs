using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.GetVolunteer;

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
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerCount = 3;

            var volunteerIds = new List<Guid>();
            for (int i = 0; i < volunteerCount; i++)
            {
                volunteerIds.Add(Guid.NewGuid());
            }

            await SeedDatabase(volunteerIds, cancellationToken);

            var idToGet = volunteerIds.Last();

            var query = new GetVolunteerByIdQuery(idToGet);


            //Act
            var result = await _sut.Handle(query, cancellationToken);

            //Arrange
            result.Should().NotBeNull();
            result.Id.Should().Be(idToGet);            
        }

        [Fact]
        public async Task Get_volunteer_by_wrong_id_from_database_return_null()
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerCount = 3;

            var volunteerIds = new List<Guid>();
            for (int i = 0; i < volunteerCount; i++)
            {
                volunteerIds.Add(Guid.NewGuid());
            }

            await SeedDatabase(volunteerIds, cancellationToken);

            var idToGet = Guid.NewGuid();

            var query = new GetVolunteerByIdQuery(idToGet);


            //Act
            var result = await _sut.Handle(query, cancellationToken);

            //Arrange
            result.Should().BeNull();            
        }

    }
}
