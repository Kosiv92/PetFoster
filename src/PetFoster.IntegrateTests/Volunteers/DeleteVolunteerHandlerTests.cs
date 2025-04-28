using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.DeleteVolunteer;
using PetFoster.Domain.Ids;

namespace PetFoster.IntegrateTests.Volunteers
{
    public class DeleteVolunteerHandlerTests : VolunteerTestBase
    {
        private readonly ICommandHandler<Guid, DeleteVolunteerCommand> _sut;

        public DeleteVolunteerHandlerTests(IntegrationTestsWebFactory factory) 
            : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
                .GetRequiredService<ICommandHandler<Guid, DeleteVolunteerCommand>>();
        }

        [Fact]
        public async Task Delete_volunteer_from_database_return_id()
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;
                       
            var volunteerId = Guid.NewGuid();

            await SeedDatabase(new List<Guid> { volunteerId }, cancellationToken);

            var command = new DeleteVolunteerCommand(volunteerId);

            //Act
            var result = await _sut.Handle(command, cancellationToken);

            //Arrange
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();
            result.Value.Should().Be(volunteerId);
        }

        [Fact]
        public async Task Delete_not_exist_volunteer_return_failure()
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerId = Guid.NewGuid();

            await SeedDatabase(new List<Guid> { Guid.NewGuid() }, cancellationToken);

            var command = new DeleteVolunteerCommand(volunteerId);

            //Act
            var result = await _sut.Handle(command, cancellationToken);

            //Arrange
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeEmpty();            
        }

        [Fact]
        public async Task Delete_volunteer_from_database_unaffected_other_volunteers()
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;

            var volunteerIdForDelete = Guid.NewGuid();
            var volunteerIdNonDeleted = Guid.NewGuid();

            await SeedDatabase(
                new List<Guid> { volunteerIdForDelete, volunteerIdNonDeleted }, 
                cancellationToken);

            var command = new DeleteVolunteerCommand(volunteerIdForDelete);
            var existVolunteer = await Repository.GetByIdAsync(
                VolunteerId.Create(volunteerIdNonDeleted), 
                cancellationToken);

            //Act
            var result = await _sut.Handle(command, cancellationToken);

            //Arrange
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();
            existVolunteer.Should().NotBeNull();
            existVolunteer.Id.Value.Should().Be(volunteerIdNonDeleted);
        }
    }
}
