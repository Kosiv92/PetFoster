using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Species.DeleteBreed;
using PetFoster.Domain.Ids;

namespace PetFoster.IntegrateTests.Breeds
{
    public class DeleteBreedHandlerTests : BreedTestBase
    {
        private readonly ICommandHandler<Guid, DeleteBreedCommand> _sut;

        public DeleteBreedHandlerTests(IntegrationTestsWebFactory factory)
            : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
                .GetRequiredService<ICommandHandler<Guid, DeleteBreedCommand>>();
        }

        [Fact]
        public async Task Delete_breed_from_database_return_success()
        {
            //Arrange
            var cancellationToken = new CancellationTokenSource().Token;
                        
            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();            

            await SeedDatabase(specieId, new List<Guid> { breedId }, cancellationToken);

            var command = new DeleteBreedCommand(specieId, breedId);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);
            var specie = await SpecieRepository
                .GetByIdAsync(
                SpecieId.Create(specieId),
                cancellationToken);
            var breed = specie.Breeds.Single();

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(breedId);
            breed.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_not_exist_breed_from_database_return_failure()
        {
            //Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();

            await SeedDatabase(specieId, new List<Guid> { breedId }, cancellationToken);

            var command = new DeleteBreedCommand(specieId, Guid.NewGuid());

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);
            var specie = await SpecieRepository
                .GetByIdAsync(
                SpecieId.Create(specieId),
                cancellationToken);
            var breed = specie.Breeds.Single();

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.NotFound);
            breed.Should().NotBeNull();
            breed.IsDeleted.Should().BeFalse();
        }
    }
}
