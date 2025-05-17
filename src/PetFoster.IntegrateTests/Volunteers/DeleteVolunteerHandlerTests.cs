using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Volunteers.DeleteVolunteer;
using PetFoster.Core.Abstractions;
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
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid volunteerId = Guid.NewGuid();

            await SeedDatabase([volunteerId], cancellationToken);

            DeleteVolunteerCommand command = new(volunteerId);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, cancellationToken);

            //Arrange
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().NotBeEmpty();
            _ = result.Value.Should().Be(volunteerId);
        }

        [Fact]
        public async Task Delete_not_exist_volunteer_return_failure()
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid volunteerId = Guid.NewGuid();

            await SeedDatabase([Guid.NewGuid()], cancellationToken);

            DeleteVolunteerCommand command = new(volunteerId);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, cancellationToken);

            //Arrange
            _ = result.IsFailure.Should().BeTrue();
            _ = result.Error.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Delete_volunteer_from_database_unaffected_other_volunteers()
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            Guid volunteerIdForDelete = Guid.NewGuid();
            Guid volunteerIdNonDeleted = Guid.NewGuid();

            await SeedDatabase(
                [volunteerIdForDelete, volunteerIdNonDeleted],
                cancellationToken);

            DeleteVolunteerCommand command = new(volunteerIdForDelete);
            Domain.Entities.Volunteer? existVolunteer = await Repository.GetByIdAsync(
                VolunteerId.Create(volunteerIdNonDeleted),
                cancellationToken);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, cancellationToken);

            //Arrange
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().NotBeEmpty();
            _ = existVolunteer.Should().NotBeNull();
            _ = existVolunteer.Id.Value.Should().Be(volunteerIdNonDeleted);
        }
    }
}
