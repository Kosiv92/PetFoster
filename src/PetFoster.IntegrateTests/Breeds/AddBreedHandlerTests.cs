using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Species.AddBreed;
using PetFoster.Core.Abstractions;
using PetFoster.Domain.Ids;

namespace PetFoster.IntegrateTests.Breeds
{
    public class AddBreedHandlerTests : BreedTestBase
    {
        private readonly ICommandHandler<Guid, AddBreedCommand> _sut;

        public AddBreedHandlerTests(IntegrationTestsWebFactory factory)
            : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
                .GetRequiredService<ICommandHandler<Guid, AddBreedCommand>>();
        }

        [Fact]
        public async Task Add_breed_to_database_return_success()
        {
            //Arrange
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid specieId = Guid.NewGuid();

            await SeedDatabase([specieId], cancellationToken);

            AddBreedCommand command = new(specieId, Fixture.Create<string>());

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, CancellationToken.None);
            Domain.Entities.Specie? specie = await SpecieRepository
                .GetByIdAsync(
                SpecieId.Create(specieId),
                cancellationToken);
            Domain.Entities.Breed breed = specie.Breeds.Single();

            //Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().NotBeEmpty();
            _ = breed.Should().NotBeNull();
            _ = breed.Id.Value.Should().Be(result.Value);
        }

        [Fact]
        public async Task Add_pet_to_not_exist_specie_return_failure()
        {
            //Arrange
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid specieId = Guid.NewGuid();

            await SeedDatabase([specieId], cancellationToken);

            AddBreedCommand command = new(Guid.NewGuid(), Fixture.Create<string>());

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            _ = result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.NotFound);
        }
    }
}
