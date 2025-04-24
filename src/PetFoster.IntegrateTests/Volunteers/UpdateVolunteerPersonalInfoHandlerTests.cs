using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.UpdatePersonalInfo;
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
            var cancellationToken = new CancellationTokenSource().Token;
            var volunteerId = Guid.NewGuid();
            await SeedDatabase(new List<Guid> { volunteerId }, cancellationToken);

            var command = CreateUpdateCommand(volunteerId);            

            //Act
            var result = await _sut.Handle(command, cancellationToken);

            //Arrange
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();
            result.Value.Should().Be(volunteerId);
        }

        [Fact]
        public async Task Update_volunteer_personal_change_personal_info_in_database()
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;
            var volunteerId = Guid.NewGuid();
            await SeedDatabase(new List<Guid> { volunteerId }, cancellationToken);

            var command = CreateUpdateCommand(volunteerId);

            //Act
            var result = await _sut.Handle(command, cancellationToken);
            var updatedVolunteer = await Repository.GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);

            //Arrange
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();
            result.Value.Should().Be(volunteerId);
            updatedVolunteer.Should().NotBeNull();
            updatedVolunteer.FullName.FirstName.Should().Be(FIRST_NAME);
            updatedVolunteer.FullName.LastName.Should().Be(LAST_NAME);
            updatedVolunteer.FullName.Patronymic.Should().Be(PATRONYMIC);
            updatedVolunteer.Email.Value.Should().Be(EMAIL);
            updatedVolunteer.PhoneNumber.Value.Should().Be(PHONE_NUMBER);
            updatedVolunteer.Description.Value.Should().Be(DESCRIPTION);
            updatedVolunteer.WorkExperienceInYears.Value.Should().Be(WORK_EXPIRIENCE);
        }

        [Fact]
        public async Task Update_not_exist_volunteer_return_failure()
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;
            var volunteerId = Guid.NewGuid();
            await SeedDatabase(new List<Guid> { volunteerId }, cancellationToken);

            var command = CreateUpdateCommand(Guid.NewGuid());

            //Act
            var result = await _sut.Handle(command, cancellationToken);
            
            //Arrange
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeEmpty();
            result.Error.First().Type.Should().Be(ErrorType.NotFound);
        }

        private UpdateVolunteerPersonalInfoCommand CreateUpdateCommand(Guid volunteerId)
        {
            var fullname = new FullNameDto(FIRST_NAME, LAST_NAME, PATRONYMIC);
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
