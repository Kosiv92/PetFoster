using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.SharedKernel;
using PetFoster.Volunteers.Application.VolunteerManagement.UpdateRequisites;
using PetFoster.Volunteers.Domain.Entities;
using PetFoster.SharedKernel.ValueObjects;

namespace PetFoster.IntegrateTests.Volunteers;

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
        CancellationToken cancellationToken = new CancellationTokenSource().Token;
        Guid volunteerId = Guid.NewGuid();
        await SeedDatabase([volunteerId], cancellationToken);

        UpdateVolunteerRequisitesCommand command = CreateUpdateVolunteerRequisitesCommand(volunteerId);

        //Act
        var result = await _sut.Handle(command, cancellationToken);
        Volunteer? updatedVolunteer = await Repository.GetByIdAsync(
            VolunteerId.Create(volunteerId),
            cancellationToken);
        AssistanceRequisites? updatedVolunteerRequisites = updatedVolunteer?
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
        CancellationToken cancellationToken = new CancellationTokenSource().Token;
        Guid volunteerId = Guid.NewGuid();
        await SeedDatabase([volunteerId], cancellationToken);

        UpdateVolunteerRequisitesCommand command = CreateUpdateVolunteerRequisitesCommand(Guid.NewGuid());

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
        List<AssistanceRequisitesDto> requisitesList = [];
        AssistanceRequisitesDto newRequisite = new(NAME, DESCRIPTION);
        requisitesList.Add(newRequisite);

        return new UpdateVolunteerRequisitesCommand(
        volunteerId, requisitesList);
    }
}