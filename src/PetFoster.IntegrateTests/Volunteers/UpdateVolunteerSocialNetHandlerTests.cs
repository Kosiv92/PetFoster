using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.UpdateSocialNet;
using PetFoster.Domain.Ids;

namespace PetFoster.IntegrateTests.Volunteers
{
    public class UpdateVolunteerSocialNetHandlerTests : VolunteerTestBase
    {
        private readonly ICommandHandler<Guid, UpdateVolunteerSocialNetCommand> _sut;
        private const string SOCIAL_NAME = "Test social name";
        private const string ACCOUNT_NAME = "Test account name";

        public UpdateVolunteerSocialNetHandlerTests(IntegrationTestsWebFactory factory) 
            : base(factory)
        {
            _sut = ServiceScope.ServiceProvider
                .GetRequiredService<ICommandHandler<Guid, UpdateVolunteerSocialNetCommand>>();
        }

        [Fact]
        public async Task Update_volunteer_requisites_return_success()
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;
            var volunteerId = Guid.NewGuid();
            await SeedDatabase(new List<Guid> { volunteerId }, cancellationToken);

            var command = CreateUpdateVolunteerSocialNetCommand(volunteerId);

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

            var command = CreateUpdateVolunteerSocialNetCommand(volunteerId);

            //Act
            var result = await _sut.Handle(command, cancellationToken);
            var updatedVolunteer = await Repository.GetByIdAsync(
                VolunteerId.Create(volunteerId),
                cancellationToken);
            var updatedVolunteerSocialNet = updatedVolunteer?.
                SocialNetContacts.First();

            //Arrange
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();
            result.Value.Should().Be(volunteerId);
            updatedVolunteer.Should().NotBeNull();
            updatedVolunteerSocialNet.Should().NotBeNull();
            updatedVolunteerSocialNet.SocialNetName.Should().Be(SOCIAL_NAME);
            updatedVolunteerSocialNet.AccountName.Should().Be(ACCOUNT_NAME);
        }

        [Fact]
        public async Task Update_not_exist_volunteer_return_failure()
        {
            //Assert
            var cancellationToken = new CancellationTokenSource().Token;
            var volunteerId = Guid.NewGuid();
            await SeedDatabase(new List<Guid> { volunteerId }, cancellationToken);

            var command = CreateUpdateVolunteerSocialNetCommand(Guid.NewGuid());

            //Act
            var result = await _sut.Handle(command, cancellationToken);

            //Arrange
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeEmpty();
            result.Error.First().Type.Should().Be(ErrorType.NotFound);
        }

        private UpdateVolunteerSocialNetCommand CreateUpdateVolunteerSocialNetCommand(
            Guid volunteerId)
        {
            var socialNetList = new List<SocialNetContactsDto>();
            var newSocialNet = new SocialNetContactsDto(SOCIAL_NAME, ACCOUNT_NAME);
            socialNetList.Add(newSocialNet);

            return new UpdateVolunteerSocialNetCommand(
            volunteerId, socialNetList);
        }
    }
}
