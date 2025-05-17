using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.Volunteers.CreateVolunteer;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
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
            VolunteerId id = VolunteerId.NewVolunteerId();

            CreateVolunteerCommand command = Fixture.CreateCreateVolunteerCommand(id);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> result = await _sut.Handle(command, CancellationToken.None);
            Domain.Entities.Volunteer? volunteer = await Repository.GetByIdAsync(id, CancellationToken.None);

            //Assert
            _ = result.IsSuccess.Should().BeTrue();
            _ = result.Value.Should().NotBeEmpty();
            _ = volunteer.Should().NotBeNull();
        }

        [Fact]
        public async Task Add_volunteers_with_same_id_to_database_return_failure()
        {
            //Arrange
            VolunteerId id = VolunteerId.NewVolunteerId();

            CreateVolunteerCommand firstCommand = Fixture.CreateCreateVolunteerCommand(id);
            CreateVolunteerCommand secondCommand = Fixture.CreateCreateVolunteerCommand(id);

            //Act
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> firstResult = await _sut.Handle(firstCommand, CancellationToken.None);
            Domain.Entities.Volunteer? volunteer = await Repository.GetByIdAsync(id, CancellationToken.None);
            CSharpFunctionalExtensions.Result<Guid, Core.ErrorList> secondResult = await _sut.Handle(secondCommand, CancellationToken.None);

            //Assert
            _ = firstResult.IsSuccess.Should().BeTrue();
            _ = firstResult.Value.Should().NotBeEmpty();
            _ = volunteer.Should().NotBeNull();
            _ = secondResult.IsFailure.Should().BeTrue();
            _ = secondResult.Error.FirstOrDefault().Code.Should().Be("record.already.exist");
        }

        protected CreateVolunteerCommand CreateCommand(VolunteerId id)
        {
            FullNameDto fullName = new("John", "Smith", "Isaac");
            string email = "anyemail@mail.com";
            string phoneNumber = "88009994488";
            string description = "Any description";
            int workExpirience = 5;
            List<AssistanceRequisitesDto> assistanceRequisitesList =
            [
                new AssistanceRequisitesDto("Requisite 1", "5480"),
                new AssistanceRequisitesDto("Requisite 2", "7615"),
            ];
            List<SocialNetContactsDto> socialNetContactsList =
            [
                new SocialNetContactsDto("Facebook", "JohnSmith.91"),
                new SocialNetContactsDto("Instagram", "johnysmith.91"),
            ];

            return new CreateVolunteerCommand(id, fullName, email,
                phoneNumber, description, workExpirience,
                assistanceRequisitesList, socialNetContactsList);
        }
    }
}
