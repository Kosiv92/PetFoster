using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Species.DeleteBreed;
using PetFoster.Core.Abstractions;
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
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid specieId = Guid.NewGuid();
            Guid breedId = Guid.NewGuid();

            await SeedDatabase(specieId, [breedId], cancellationToken);

            DeleteBreedCommand command = new(specieId, breedId);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, CancellationToken.None);
            Domain.Entities.Specie? specie = await SpecieRepository
                .GetByIdAsync(
                SpecieId.Create(specieId),
                cancellationToken);
            Domain.Entities.Breed breed = specie.Breeds.Single();

            //Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().Be(breedId);
            _ = breed.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_not_exist_breed_from_database_return_failure()
        {
            //Arrange
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid specieId = Guid.NewGuid();
            Guid breedId = Guid.NewGuid();

            await SeedDatabase(specieId, [breedId], cancellationToken);

            DeleteBreedCommand command = new(specieId, Guid.NewGuid());

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, CancellationToken.None);
            Domain.Entities.Specie? specie = await SpecieRepository
                .GetByIdAsync(
                SpecieId.Create(specieId),
                cancellationToken);
            Domain.Entities.Breed breed = specie.Breeds.Single();

            //Assert
            _ = result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.NotFound);
            _ = breed.Should().NotBeNull();
            _ = breed.IsDeleted.Should().BeFalse();
        }
    }
}
