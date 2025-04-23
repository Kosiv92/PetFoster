using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.CreateVolunteer;
using PetFoster.Domain.Ids;

namespace PetFoster.IntegrateTests.Volunteers
{
    public class CreateVolunteerHandlerTests 
        : VolunteerTestBase
    {        
        private readonly ICommandHandler<Guid, CreateVolunteerCommand> _sut;

        public CreateVolunteerHandlerTests(IntegrationTestsWebFactory factory) 
            : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
                .GetRequiredService<ICommandHandler<Guid, CreateVolunteerCommand>>();
        }    

        [Fact]
        public async Task Add_volunteer_to_database_return_success()
        {
            //Arrange
            var id = VolunteerId.NewVolunteerId();
            
            var command = Fixture.CreateCreateVolunteerCommand(id);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);
            var volunteer = await Repository.GetByIdAsync(id, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();            
            volunteer.Should().NotBeNull();
        }

        [Fact]
        public async Task Add_volunteers_with_same_id_to_database_return_failure()
        {
            //Arrange
            var id = VolunteerId.NewVolunteerId();

            var firstCommand = Fixture.CreateCreateVolunteerCommand(id);
            var secondCommand = Fixture.CreateCreateVolunteerCommand(id);

            //Act
            var firstResult = await _sut.Handle(firstCommand, CancellationToken.None);
            var volunteer = await Repository.GetByIdAsync(id, CancellationToken.None);
            var secondResult = await _sut.Handle(secondCommand, CancellationToken.None);

            //Assert
            firstResult.IsSuccess.Should().BeTrue();
            firstResult.Value.Should().NotBeEmpty();
            volunteer.Should().NotBeNull();
            secondResult.IsFailure.Should().BeTrue();
            secondResult.Error.FirstOrDefault().Code.Should().Be("record.already.exist");
        }
    }
}
