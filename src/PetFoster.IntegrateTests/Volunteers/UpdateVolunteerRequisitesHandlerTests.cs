using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.UpdateRequisites;
using PetFoster.Domain.Ids;

namespace PetFoster.IntegrateTests.Volunteers
{
    public class UpdateVolunteerRequisitesHandlerTests : VolunteerTestBase
    {
        private readonly ICommandHandler<Guid, UpdateVolunteerRequisitesCommand> _sut;
        private const string NAME = "Test name";
        private const string DESCRIPTION = "Test description";       

        public UpdateVolunteerRequisitesHandlerTests(IntegrationTestsWebFactory factory)
            : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
               .GetRequiredService<ICommandHandler<Guid, UpdateVolunteerRequisitesCommand>>();
        }

        [Fact]
        public async Task Update_volunteer_requisites_return_success()
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;
            var volunteerId = Guid.NewGuid();
            await SeedDatabase(new List<Guid> { volunteerId }, cancellationToken);

            var command = CreateUpdateVolunteerRequisitesCommand(volunteerId);

            //Act
            var result = await _sut.Handle(command, cancellationToken);

            //Arrange
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();
            result.Value.Should().Be(volunteerId);
        }

        [Fact]
        public async Task Update_volunteer_change_personal_info_in_database()
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;
            var volunteerId = Guid.NewGuid();
            await SeedDatabase(new List<Guid> { volunteerId }, cancellationToken);

            var command = CreateUpdateVolunteerRequisitesCommand(volunteerId);

            //Act
            var result = await _sut.Handle(command, cancellationToken);
            var updatedVolunteer = await Repository.GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);
            var updatedVolunteerRequisites = updatedVolunteer?
                .AssistanceRequisitesList.First();

            //Arrange
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();
            result.Value.Should().Be(volunteerId);
            updatedVolunteer.Should().NotBeNull();
            updatedVolunteerRequisites.Should().NotBeNull();
            updatedVolunteerRequisites.Name.Should().Be(NAME);
            updatedVolunteerRequisites.Description.Value.Should().Be(DESCRIPTION);
        }

        [Fact]
        public async Task Update_not_exist_volunteer_return_failure()
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;
            var volunteerId = Guid.NewGuid();
            await SeedDatabase(new List<Guid> { volunteerId }, cancellationToken);

            var command = CreateUpdateVolunteerRequisitesCommand(Guid.NewGuid());

            //Act
            var result = await _sut.Handle(command, cancellationToken);

            //Arrange
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeEmpty();
            result.Error.First().Type.Should().Be(ErrorType.NotFound);
        }


        private UpdateVolunteerRequisitesCommand CreateUpdateVolunteerRequisitesCommand(
            Guid volunteerId)
        {
            var requisitesList = new List<AssistanceRequisitesDto>();
            var newRequisite = new AssistanceRequisitesDto(NAME, DESCRIPTION);
            requisitesList.Add(newRequisite);

            return new UpdateVolunteerRequisitesCommand(
            volunteerId, requisitesList);
        }
    }
}