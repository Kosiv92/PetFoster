using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.SharedKernel;
using PetFoster.Volunteers.Application.VolunteerManagement.UpdateSocialNet;
using PetFoster.Volunteers.Domain.Entities;
using PetFoster.SharedKernel.ValueObjects;

namespace PetFoster.IntegrateTests.Volunteers;

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
        CancellationToken cancellationToken = new CancellationTokenSource().Token;
        Guid volunteerId = Guid.NewGuid();
        await SeedDatabase([volunteerId], cancellationToken);

        UpdateVolunteerSocialNetCommand command = CreateUpdateVolunteerSocialNetCommand(volunteerId);

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

        UpdateVolunteerSocialNetCommand command = CreateUpdateVolunteerSocialNetCommand(volunteerId);

        //Act
        var result = await _sut.Handle(command, cancellationToken);
        Volunteer? updatedVolunteer = await Repository.GetByIdAsync(
            VolunteerId.Create(volunteerId),
            cancellationToken);
        SocialNetContact? updatedVolunteerSocialNet = updatedVolunteer?.
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
        CancellationToken cancellationToken = new CancellationTokenSource().Token;
        Guid volunteerId = Guid.NewGuid();
        await SeedDatabase([volunteerId], cancellationToken);

        UpdateVolunteerSocialNetCommand command = CreateUpdateVolunteerSocialNetCommand(Guid.NewGuid());

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
        List<SocialNetContactsDto> socialNetList = [];
        SocialNetContactsDto newSocialNet = new(SOCIAL_NAME, ACCOUNT_NAME);
        socialNetList.Add(newSocialNet);

        return new UpdateVolunteerSocialNetCommand(
        volunteerId, socialNetList);
    }
}
