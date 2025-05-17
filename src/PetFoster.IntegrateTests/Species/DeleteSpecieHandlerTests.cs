using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Species.DeleteSpecie;
using PetFoster.Core.Abstractions;
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
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid specieId = Guid.NewGuid();

            await SeedDatabase([specieId], cancellationToken);

            DeleteSpecieCommand command = new(specieId);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, cancellationToken);

            //Arrange
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().NotBeEmpty();
            _ = result.Value.Should().Be(specieId);
        }

        [Fact]
        public async Task Delete_not_exist_specie_return_failure()
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid specieId = Guid.NewGuid();

            await SeedDatabase([specieId], cancellationToken);

            DeleteSpecieCommand command = new(Guid.NewGuid());

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, cancellationToken);

            //Arrange
            _ = result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task Delete_linked_specie_return_failure()
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid specieId = Guid.NewGuid();
            Guid breedId = Guid.NewGuid();
            Guid volunteerId = Guid.NewGuid();
            Guid petId = Guid.NewGuid();

            await SeedDatabase(specieId, breedId, volunteerId,
                petId, cancellationToken);

            DeleteSpecieCommand command = new(specieId);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, cancellationToken);
            Domain.Entities.Specie? specie = await SpecieRepository.GetByIdAsync(
                SpecieId.Create(specieId),
                cancellationToken);

            //Arrange
            _ = result.IsFailure.Should().BeTrue();
            result.Error.Single().Type.Should().Be(ErrorType.Conflict);
            _ = specie.Should().NotBeNull();
        }
    }
}
