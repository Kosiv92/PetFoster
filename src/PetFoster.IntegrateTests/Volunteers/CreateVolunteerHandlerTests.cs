using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Application.DTO.Volunteer;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.CreateVolunteer;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;

namespace PetFoster.IntegrateTests.Volunteers
{
    public class CreateVolunteerHandlerTests 
        : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
    {
        private readonly IntegrationTestsWebFactory _factory;
        private readonly Fixture _fixture;
        private readonly IServiceScope _serviceScope;
        private readonly IRepository<Volunteer, VolunteerId> _repository;
        private readonly ICommandHandler<Guid, CreateVolunteerCommand> _sut;

        public CreateVolunteerHandlerTests(IntegrationTestsWebFactory factory)
        {
            _factory = factory;
            _fixture = new Fixture();
            _serviceScope = _factory.Services.CreateScope();
            _repository = _serviceScope.ServiceProvider
                .GetRequiredService<IRepository<Volunteer, VolunteerId>>();
            _sut = _serviceScope.ServiceProvider
                .GetRequiredService<ICommandHandler<Guid, CreateVolunteerCommand>>();
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public Task DisposeAsync()
        {
            _serviceScope.Dispose();            
            return _factory.ResetDataBaseAsync();
        }        

        [Fact]
        public async Task Add_volunteer_to_database_return_success()
        {
            //Arrange
            var id = VolunteerId.NewVolunteerId();
            
            var command = _fixture.CreateCreateVolunteerCommand(id);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);
            var volunteer = await _repository.GetByIdAsync(id, CancellationToken.None);

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

            var firstCommand = _fixture.CreateCreateVolunteerCommand(id);
            var secondCommand = _fixture.CreateCreateVolunteerCommand(id);

            //Act
            var firstResult = await _sut.Handle(firstCommand, CancellationToken.None);
            var volunteer = await _repository.GetByIdAsync(id, CancellationToken.None);
            var secondResult = await _sut.Handle(secondCommand, CancellationToken.None);

            //Assert
            firstResult.IsSuccess.Should().BeTrue();
            firstResult.Value.Should().NotBeEmpty();
            volunteer.Should().NotBeNull();
            secondResult.IsFailure.Should().BeTrue();
            secondResult.Error.FirstOrDefault().Code.Should().Be("record.already.exist");
        }

        private CreateVolunteerCommand CreateCommand(VolunteerId id)
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
