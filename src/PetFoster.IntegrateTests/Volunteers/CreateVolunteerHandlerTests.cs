using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.DTO.Volunteer;
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

        protected CreateVolunteerCommand CreateCommand(VolunteerId id)
        {
            var fullName = new FullNameDto("John", "Smith", "Isaac");
            var email = "anyemail@mail.com";
            var phoneNumber = "88009994488";
            var description = "Any description";
            var workExpirience = 5;
            var assistanceRequisitesList = new List<AssistanceRequisitesDto>()
            {
                new AssistanceRequisitesDto("Requisite 1", "5480"),
                new AssistanceRequisitesDto("Requisite 2", "7615"),
            };
            var socialNetContactsList = new List<SocialNetContactsDto>()
            {
                new SocialNetContactsDto("Facebook", "JohnSmith.91"),
                new SocialNetContactsDto("Instagram", "johnysmith.91"),
            };

            return new CreateVolunteerCommand(id, fullName, email,
                phoneNumber, description, workExpirience,
                assistanceRequisitesList, socialNetContactsList);
        }
    }
}
