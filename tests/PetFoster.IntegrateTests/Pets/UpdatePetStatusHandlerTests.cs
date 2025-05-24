using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Core.Abstractions;
using PetFoster.SharedKernel.Enums;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.SharedKernel;
using PetFoster.Volunteers.Application.PetManagement.UpdatePetStatus;
using PetFoster.Volunteers.Domain.Entities;

namespace PetFoster.IntegrateTests.Pets;

public class UpdatePetStatusHandlerTests : PetTestBase
{
    private readonly ICommandHandler<Guid, UpdatePetStatusCommand> _sut;

    public UpdatePetStatusHandlerTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = ServiceScope.ServiceProvider
            .GetRequiredService<ICommandHandler<Guid, UpdatePetStatusCommand>>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task Update_valid_pet_status_return_success(int assistanceStatusCode)
    {
        //Arrange
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        Guid volunteerId = Guid.NewGuid();
        Guid specieId = Guid.NewGuid();
        Guid breedId = Guid.NewGuid();
        Guid petId = Guid.NewGuid();

        await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

        string assistanceStatus = ((AssistanceStatus)assistanceStatusCode).ToString();

        UpdatePetStatusCommand command = new(volunteerId, petId, assistanceStatus);

        //Act
        var result = await _sut.Handle(command, CancellationToken.None);
        Volunteer? volunteer = await VolunteerRepository
            .GetByIdAsync(
            VolunteerId.Create(volunteerId),
            cancellationToken);
        Pet pet = volunteer!.FosteredAnimals.Single();

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(petId);
        pet.Id.Value.Should().Be(petId);
        pet.AssistanceStatus.Should().Be((AssistanceStatus)assistanceStatusCode);
    }

    [Fact]
    public async Task Update_invalid_pet_status_return_failure()
    {
        //Arrange
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        Guid volunteerId = Guid.NewGuid();
        Guid specieId = Guid.NewGuid();
        Guid breedId = Guid.NewGuid();
        Guid petId = Guid.NewGuid();

        await SeedDatabase(volunteerId, specieId, breedId, petId, cancellationToken);

        AssistanceStatus assistanceStatus = AssistanceStatus.FoundHome;

        UpdatePetStatusCommand command = new(volunteerId, petId, assistanceStatus.ToString());

        //Act
        var result = await _sut.Handle(command, CancellationToken.None);
        Volunteer? volunteer = await VolunteerRepository
            .GetByIdAsync(
            VolunteerId.Create(volunteerId),
            cancellationToken);
        Pet pet = volunteer!.FosteredAnimals.Single();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Single().Type.Should().Be(ErrorType.Validation);
        pet.Id.Value.Should().Be(petId);
        pet.AssistanceStatus.Should().NotBe(assistanceStatus);
    }

}
