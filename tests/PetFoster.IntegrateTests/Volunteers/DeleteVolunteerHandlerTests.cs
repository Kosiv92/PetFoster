using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Core.Abstractions;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Volunteers.Application.VolunteerManagement.DeleteVolunteer;
using PetFoster.Volunteers.Domain.Entities;

namespace PetFoster.IntegrateTests.Volunteers;

public class DeleteVolunteerHandlerTests : VolunteerTestBase
{
    private readonly ICommandHandler<Guid, DeleteVolunteerCommand> _sut;

    public DeleteVolunteerHandlerTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = ServiceScope.ServiceProvider
            .GetRequiredService<ICommandHandler<Guid, DeleteVolunteerCommand>>();
    }

    [Fact]
    public async Task Delete_volunteer_from_database_return_id()
    {
        //Assert
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        Guid volunteerId = Guid.NewGuid();

        await SeedDatabase([volunteerId], cancellationToken);

        DeleteVolunteerCommand command = new(volunteerId);

        //Act
        var result = await _sut.Handle(command, cancellationToken);

        //Arrange
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value.Should().Be(volunteerId);
    }

    [Fact]
    public async Task Delete_not_exist_volunteer_return_failure()
    {
        //Assert
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        Guid volunteerId = Guid.NewGuid();

        await SeedDatabase([Guid.NewGuid()], cancellationToken);

        DeleteVolunteerCommand command = new(volunteerId);

        //Act
        var result = await _sut.Handle(command, cancellationToken);

        //Arrange
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Delete_volunteer_from_database_unaffected_other_volunteers()
    {
        //Assert
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        Guid volunteerIdForDelete = Guid.NewGuid();
        Guid volunteerIdNonDeleted = Guid.NewGuid();

        await SeedDatabase(
            [volunteerIdForDelete, volunteerIdNonDeleted],
            cancellationToken);

        DeleteVolunteerCommand command = new(volunteerIdForDelete);
        Volunteer? existVolunteer = await Repository.GetByIdAsync(
            VolunteerId.Create(volunteerIdNonDeleted),
            cancellationToken);

        //Act
        var result = await _sut.Handle(command, cancellationToken);

        //Arrange
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        existVolunteer.Should().NotBeNull();
        existVolunteer.Id.Value.Should().Be(volunteerIdNonDeleted);
    }
}
