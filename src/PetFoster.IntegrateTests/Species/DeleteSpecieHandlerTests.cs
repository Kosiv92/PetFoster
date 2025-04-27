using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Species.DeleteSpecie;
using PetFoster.Domain.Ids;

namespace PetFoster.IntegrateTests.Species
{
    public class DeleteSpecieHandlerTests : SpecieTestBase
    {
        private readonly ICommandHandler<Guid, DeleteSpecieCommand> _sut;

        public DeleteSpecieHandlerTests(IntegrationTestsWebFactory factory) : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
                .GetRequiredService<ICommandHandler<Guid, DeleteSpecieCommand>>();
        }

        [Fact]
        public async Task Delete_volunteer_from_database_return_id()
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;

            var specieId = Guid.NewGuid();

            await SeedDatabase(new List<Guid>{ specieId }, cancellationToken);

            var command = new DeleteSpecieCommand(specieId);

            //Act
            var result = await _sut.Handle(command, cancellationToken);

            //Arrange
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();
            result.Value.Should().Be(specieId);
        }

        [Fact]
        public async Task Delete_not_exist_specie_return_failure()
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;

            var specieId = Guid.NewGuid();

            await SeedDatabase(new List<Guid> { specieId }, cancellationToken);

            var command = new DeleteSpecieCommand(Guid.NewGuid());

            //Act
            var result = await _sut.Handle(command, cancellationToken);

            //Arrange
            result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.NotFound);            
        }

        [Fact]
        public async Task Delete_linked_specie_return_failure()
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;

            var specieId = Guid.NewGuid();
            var breedId = Guid.NewGuid();
            var volunteerId = Guid.NewGuid();
            var petId = Guid.NewGuid();

            await SeedDatabase(specieId, breedId, volunteerId, 
                petId, cancellationToken);

            var command = new DeleteSpecieCommand(specieId);

            //Act
            var result = await _sut.Handle(command, cancellationToken);
            var specie = await SpecieRepository.GetByIdAsync(
                SpecieId.Create(specieId), 
                cancellationToken);

            //Arrange
            result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.Conflict);
            specie.Should().NotBeNull();
        }
    }
}
