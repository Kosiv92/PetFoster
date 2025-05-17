using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.Extensions;
using PetFoster.Core.Interfaces;
using PetFoster.Species.Domain.Entities;
using PetFoster.Species.Domain.Ids;
using PetFoster.Volunteers.Application.Interfaces;
using PetFoster.Volunteers.Application.PetManagement.GetPetsBySpecieId;

namespace PetFoster.Species.Application.SpecieManagement.DeleteSpecie;

public sealed class DeleteSpecieHandler : ICommandHandler<Guid, DeleteSpecieCommand>
{
    private readonly IRepository<Specie, SpecieId> _repository;
    private readonly IVolunteersQueryRepository _volunteersQueryRepository;
    private readonly IValidator<DeleteSpecieCommand> _validator;
    private readonly ILogger<DeleteSpecieHandler> _logger;

    public DeleteSpecieHandler(IRepository<Specie, SpecieId> repository,
        IVolunteersQueryRepository volunteersQueryRepository,
        IValidator<DeleteSpecieCommand> validator, ILogger<DeleteSpecieHandler> logger)
    {
        _repository = repository;
        _volunteersQueryRepository = volunteersQueryRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(DeleteSpecieCommand command,
        CancellationToken cancellationToken = default)
    {
        FluentValidation.Results.ValidationResult validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        SpecieId id = SpecieId.Create(command.Id);

        Specie? specieForDelete = await _repository.GetByIdAsync(id, cancellationToken);

        if (specieForDelete == null)
        {
            return Errors.General.NotFound(command.Id).ToErrorList();
        }

        GetPetsBySpecieIdQuery query = new(specieForDelete.Id);

        IEnumerable<PetDto> speciesPets = await _volunteersQueryRepository.GetPetsBySpecieId(query, cancellationToken);

        if (speciesPets?.Any() == true)
        {
            return Errors.General.ReferenceExist().ToErrorList();
        }

        specieForDelete.Delete();

        await _repository.SaveChangesAsync(specieForDelete, cancellationToken);

        _logger.LogInformation("Deleted specie {SpecieName} with id {SpecieId}",
            specieForDelete.Name, specieForDelete.Id);

        return (Guid)specieForDelete.Id;
    }
}
