using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Species.CreateSpecie;
using PetFoster.Core.Abstractions;
using PetFoster.Domain.Ids;

namespace PetFoster.IntegrateTests.Species
{
    public class CreateSpecieHandlerTests : SpecieTestBase
    {
        private readonly ICommandHandler<Guid, CreateSpecieCommand> _sut;

        public CreateSpecieHandlerTests(IntegrationTestsWebFactory factory)
            : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
                .GetRequiredService<ICommandHandler<Guid, CreateSpecieCommand>>();
        }

        [Fact]
        public async Task Add_specie_to_database_return_success()
        {
            //Arrange
            SpecieId id = SpecieId.NewSpecieId();
            CreateSpecieCommand command = new(id, Fixture.Create<string>());

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, CancellationToken.None);
            Domain.Entities.Specie? specie = await SpecieRepository.GetByIdAsync(
                SpecieId.Create(id),
                CancellationToken.None);

            //Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().NotBeEmpty();
            _ = specie.Should().NotBeNull();
        }

        [Fact]
        public async Task Add_species_with_same_id_to_database_return_failure()
        {
            //Arrange
            SpecieId id = SpecieId.NewSpecieId();

            CreateSpecieCommand firstCommand = new(id, Fixture.Create<string>());
            CreateSpecieCommand secondCommand = new(id, Fixture.Create<string>());

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> firstResult = await _sut.Handle(firstCommand, CancellationToken.None);
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> secondResult = await _sut.Handle(secondCommand, CancellationToken.None);

            //Assert
            _ = firstResult.IsSuccess.Should().BeTrue();
            _ = firstResult.Value.Should().NotBeEmpty();
            _ = secondResult.IsFailure.Should().BeTrue();
            secondResult.Error.Single().Type.Should().Be(ErrorType.Validation);
        }
    }
}
