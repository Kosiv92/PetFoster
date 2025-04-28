using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Species.CreateSpecie;
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
            var id = SpecieId.NewSpecieId();
            var command = new CreateSpecieCommand(id, Fixture.Create<string>());

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);
            var specie = await SpecieRepository.GetByIdAsync(
                SpecieId.Create(id), 
                CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();
            specie.Should().NotBeNull();
        }

        [Fact]
        public async Task Add_species_with_same_id_to_database_return_failure()
        {
            //Arrange
            var id = SpecieId.NewSpecieId();

            var firstCommand = new CreateSpecieCommand(id, Fixture.Create<string>());
            var secondCommand = new CreateSpecieCommand(id, Fixture.Create<string>());

            //Act
            var firstResult = await _sut.Handle(firstCommand, CancellationToken.None);            
            var secondResult = await _sut.Handle(secondCommand, CancellationToken.None);            

            //Assert
            firstResult.IsSuccess.Should().BeTrue();
            firstResult.Value.Should().NotBeEmpty();
            secondResult.IsFailure.Should().BeTrue();
            secondResult.Error.Single().Type.Should().Be(ErrorType.Validation);            
        }
    }
}
