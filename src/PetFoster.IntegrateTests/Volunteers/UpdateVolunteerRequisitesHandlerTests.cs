using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Volunteers.UpdateRequisites;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
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
            CancellationToken cancellationToken = new CancellationTokenSource().Token;
            Guid volunteerId = Guid.NewGuid();
            await SeedDatabase([volunteerId], cancellationToken);

            UpdateVolunteerRequisitesCommand command = CreateUpdateVolunteerRequisitesCommand(volunteerId);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, cancellationToken);

            //Arrange
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().NotBeEmpty();
            _ = result.Value.Should().Be(volunteerId);
        }

        [Fact]
        public async Task Update_volunteer_change_personal_info_in_database()
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;
            Guid volunteerId = Guid.NewGuid();
            await SeedDatabase([volunteerId], cancellationToken);

            UpdateVolunteerRequisitesCommand command = CreateUpdateVolunteerRequisitesCommand(volunteerId);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, cancellationToken);
            Domain.Entities.Volunteer? updatedVolunteer = await Repository.GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);
            Core.ValueObjects.AssistanceRequisites? updatedVolunteerRequisites = updatedVolunteer?
                .AssistanceRequisitesList.First();

            //Arrange
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().NotBeEmpty();
            _ = result.Value.Should().Be(volunteerId);
            _ = updatedVolunteer.Should().NotBeNull();
            _ = updatedVolunteerRequisites.Should().NotBeNull();
            _ = updatedVolunteerRequisites.Name.Should().Be(NAME);
            _ = updatedVolunteerRequisites.Description.Value.Should().Be(DESCRIPTION);
        }

        [Fact]
        public async Task Update_not_exist_volunteer_return_failure()
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;
            Guid volunteerId = Guid.NewGuid();
            await SeedDatabase([volunteerId], cancellationToken);

            UpdateVolunteerRequisitesCommand command = CreateUpdateVolunteerRequisitesCommand(Guid.NewGuid());

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, cancellationToken);

            //Arrange
            _ = result.IsFailure.Should().BeTrue();
            _ = result.Error.Should().NotBeEmpty();
            result.Error.First().Type.Should().Be(ErrorType.NotFound);
        }


        private UpdateVolunteerRequisitesCommand CreateUpdateVolunteerRequisitesCommand(
            Guid volunteerId)
        {
            List<AssistanceRequisitesDto> requisitesList = [];
            AssistanceRequisitesDto newRequisite = new(NAME, DESCRIPTION);
            requisitesList.Add(newRequisite);

            return new UpdateVolunteerRequisitesCommand(
            volunteerId, requisitesList);
        }
    }
}