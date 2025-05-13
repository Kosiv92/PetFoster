using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.GetPetByID;

namespace PetFoster.IntegrateTests.Pets
{
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
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerId = Guid.NewGuid();
            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();
            var petId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            var query = new GetPetByIdQuery(volunteerId, petId);

            //Act
            var result = await _sut.Handle(query, CancellationToken.None);
            

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(petId);            
        }

        [Fact]
        public async Task Get_not_exist_pet_from_database_return_null()
        {
            //Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerId = Guid.NewGuid();
            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();
            var petId = Guid.NewGuid();

            await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

            var query = new GetPetByIdQuery(volunteerId, Guid.NewGuid());

            //Act
            var result = await _sut.Handle(query, CancellationToken.None);


            //Assert
            result.Should().BeNull();            
        }
    }
}
