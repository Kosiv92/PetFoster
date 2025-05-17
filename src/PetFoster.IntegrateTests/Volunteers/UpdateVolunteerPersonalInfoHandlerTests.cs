using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Volunteers.UpdatePersonalInfo;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Domain.Ids;

namespace PetFoster.IntegrateTests.Volunteers
{
    public class UpdateVolunteerPersonalInfoHandlerTests : VolunteerTestBase
    {
        private readonly ICommandHandler<Guid, UpdateVolunteerPersonalInfoCommand> _sut;
        private const string FIRST_NAME = "TestFirstName";
        private const string LAST_NAME = "TestLastName";
        private const string PATRONYMIC = "TestPatronymic";
        private const string PHONE_NUMBER = "88888888888";
        private const string EMAIL = "test@test.com";
        private const string DESCRIPTION = "Test description";
        private const int WORK_EXPIRIENCE = 19;

        public UpdateVolunteerPersonalInfoHandlerTests(IntegrationTestsWebFactory factory)
            : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
                .GetRequiredService<ICommandHandler<Guid, UpdateVolunteerPersonalInfoCommand>>();
        }

        [Fact]
        public async Task Update_volunteer_personal_info_return_success()
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;
            Guid volunteerId = Guid.NewGuid();
            await SeedDatabase([volunteerId], cancellationToken);

            UpdateVolunteerPersonalInfoCommand command = CreateUpdateCommand(volunteerId);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, cancellationToken);

            //Arrange
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().NotBeEmpty();
            _ = result.Value.Should().Be(volunteerId);
        }

        [Fact]
        public async Task Update_volunteer_personal_change_personal_info_in_database()
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;
            Guid volunteerId = Guid.NewGuid();
            await SeedDatabase([volunteerId], cancellationToken);

            UpdateVolunteerPersonalInfoCommand command = CreateUpdateCommand(volunteerId);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, cancellationToken);
            Domain.Entities.Volunteer? updatedVolunteer = await Repository.GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);

            //Arrange
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().NotBeEmpty();
            _ = result.Value.Should().Be(volunteerId);
            _ = updatedVolunteer.Should().NotBeNull();
            _ = updatedVolunteer.FullName.FirstName.Should().Be(FIRST_NAME);
            _ = updatedVolunteer.FullName.LastName.Should().Be(LAST_NAME);
            _ = updatedVolunteer.FullName.Patronymic.Should().Be(PATRONYMIC);
            _ = updatedVolunteer.Email.Value.Should().Be(EMAIL);
            _ = updatedVolunteer.PhoneNumber.Value.Should().Be(PHONE_NUMBER);
            _ = updatedVolunteer.Description.Value.Should().Be(DESCRIPTION);
            _ = updatedVolunteer.WorkExperienceInYears.Value.Should().Be(WORK_EXPIRIENCE);
        }

        [Fact]
        public async Task Update_not_exist_volunteer_return_failure()
        {
            //Assert
            CancellationToken cancellationToken = new CancellationTokenSource().Token;
            Guid volunteerId = Guid.NewGuid();
            await SeedDatabase([volunteerId], cancellationToken);

            UpdateVolunteerPersonalInfoCommand command = CreateUpdateCommand(Guid.NewGuid());

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, cancellationToken);

            //Arrange
            _ = result.IsFailure.Should().BeTrue();
            _ = result.Error.Should().NotBeEmpty();
            result.Error.First().Type.Should().Be(ErrorType.NotFound);
        }

        private UpdateVolunteerPersonalInfoCommand CreateUpdateCommand(Guid volunteerId)
        {
            FullNameDto fullname = new(FIRST_NAME, LAST_NAME, PATRONYMIC);
            return new UpdateVolunteerPersonalInfoCommand(
                volunteerId,
                fullname,
                EMAIL,
                PHONE_NUMBER,
                DESCRIPTION,
                WORK_EXPIRIENCE);
        }
    }
}
