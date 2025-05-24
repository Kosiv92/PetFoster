using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Volunteers.Application.VolunteerManagement.CreateVolunteer;
using PetFoster.Volunteers.Domain.Entities;

namespace PetFoster.IntegrateTests.Volunteers;

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
        var result = await _sut.Handle(command, CancellationToken.None);
        Volunteer? volunteer = await Repository.GetByIdAsync(id, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        volunteer.Should().NotBeNull();
    }

    [Fact]
    public async Task Add_volunteers_with_same_id_to_database_return_failure()
    {
        //Arrange
        VolunteerId id = VolunteerId.NewVolunteerId();

        CreateVolunteerCommand firstCommand = Fixture.CreateCreateVolunteerCommand(id);
        CreateVolunteerCommand secondCommand = Fixture.CreateCreateVolunteerCommand(id);

        //Act
        var firstResult = await _sut.Handle(firstCommand, CancellationToken.None);
        Volunteer? volunteer = await Repository.GetByIdAsync(id, CancellationToken.None);
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
