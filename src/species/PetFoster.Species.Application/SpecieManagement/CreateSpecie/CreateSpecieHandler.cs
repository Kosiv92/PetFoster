using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Species.Domain.Entities;

namespace PetFoster.Species.Application.SpecieManagement.CreateSpecie;

public sealed class CreateSpecieHandler
    : ICommandHandler<Guid, CreateSpecieCommand>
{
    private readonly IRepository<Specie, SpecieId> _repository;
    private readonly IValidator<CreateSpecieCommand> _validator;
    private readonly ILogger<CreateSpecieHandler> _logger;

    public CreateSpecieHandler(IRepository<Specie, SpecieId> repository,
        IValidator<CreateSpecieCommand> validator,
        ILogger<CreateSpecieHandler> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(CreateSpecieCommand command,
        CancellationToken cancellationToken = default)
    {
        FluentValidation.Results.ValidationResult validationResult
            = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        SpecieName specieName = SpecieName.Create(command.Name).Value;

        Specie? exitSpecie = await _repository
            .GetByIdAsync(SpecieId.Create(command.Id),
            cancellationToken);

        if (exitSpecie != null)
        {
            return Errors.Volunteer.AlreadyExist().ToErrorList();
        }

        SpecieId specieId = SpecieId.Create(command.Id);

        Specie newSpecie = new(specieId, specieName);

        await _repository.AddAsync(newSpecie, cancellationToken);

        _logger.LogInformation("Created specie {SpecieName} with id {SpecieId}",
        newSpecie.Name.Value, newSpecie.Id.Value);

        return (Guid)newSpecie.Id;
    }
}
