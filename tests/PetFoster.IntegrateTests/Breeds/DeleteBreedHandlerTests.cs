﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFoster.Core.Abstractions;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.SharedKernel;
using PetFoster.Species.Application.BreedManagement.DeleteBreed;
using PetFoster.Species.Domain.Entities;

namespace PetFoster.IntegrateTests.Breeds;

public class DeleteBreedHandlerTests : BreedTestBase
{
    private readonly ICommandHandler<Guid, DeleteBreedCommand> _sut;

    public DeleteBreedHandlerTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = ServiceScope.ServiceProvider
            .GetRequiredService<ICommandHandler<Guid, DeleteBreedCommand>>();
    }

    [Fact]
    public async Task Delete_breed_from_database_return_success()
    {
        //Arrange
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        Guid specieId = Guid.NewGuid();
        Guid breedId = Guid.NewGuid();

        await SeedDatabase(specieId, [breedId], cancellationToken);

        DeleteBreedCommand command = new(specieId, breedId);

        //Act
        var result = await _sut.Handle(command, CancellationToken.None);
        Specie? specie = await SpecieRepository
            .GetByIdAsync(
            SpecieId.Create(specieId),
            cancellationToken);
        Breed breed = specie.Breeds.Single();

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(breedId);
        breed.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_not_exist_breed_from_database_return_failure()
    {
        //Arrange
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        Guid specieId = Guid.NewGuid();
        Guid breedId = Guid.NewGuid();

        await SeedDatabase(specieId, [breedId], cancellationToken);

        DeleteBreedCommand command = new(specieId, Guid.NewGuid());

        //Act
        var result = await _sut.Handle(command, CancellationToken.None);
        Specie? specie = await SpecieRepository
            .GetByIdAsync(
            SpecieId.Create(specieId),
            cancellationToken);
        Breed breed = specie.Breeds.Single();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Single().Type.Should().Be(ErrorType.NotFound);
        breed.Should().NotBeNull();
        breed.IsDeleted.Should().BeFalse();
    }
}
