using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.Core.Interfaces;
using PetFoster.Species.Domain.Entities;
using PetFoster.Species.Domain.Ids;
using PetFoster.Core.DTO.Volunteer;

namespace PetFoster.Species.Application.BreedManagement.DeleteBreed;

public sealed class DeleteBreedHandler : ICommandHandler<Guid, DeleteBreedCommand>
{
    private readonly IRepository<Specie, SpecieId> _repository;
    private readonly IVolunteersQueryRepository _volunteersQueryRepository;
    private readonly IValidator<DeleteBreedCommand> _validator;
    private readonly ILogger<DeleteBreedHandler> _logger;

    public DeleteBreedHandler(IRepository<Specie, SpecieId> repository,
        IVolunteersQueryRepository volunteersQueryRepository,
        IValidator<DeleteBreedCommand> validator,
        ILogger<DeleteBreedHandler> logger)
    {
        _repository = repository;
        _volunteersQueryRepository = volunteersQueryRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(DeleteBreedCommand command,
        CancellationToken cancellationToken = default)
    {
        FluentValidation.Results.ValidationResult validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        SpecieId specieId = SpecieId.Create(command.SpecieId);
        BreedId breedId = BreedId.Create(command.BreedId);

        Specie? specie = await _repository.GetByIdAsync(specieId, cancellationToken);

        if (specie == null)
        {
            return Errors.General.NotFound(command.SpecieId).ToErrorList();
        }

        Breed? breedForDelete = specie.Breeds.FirstOrDefault(b => b.Id == breedId);

        if (breedForDelete == null)
        {
            return Errors.General.NotFound(command.BreedId).ToErrorList();
        }

        GetPetsByBreedIdQuery query = new(breedForDelete.Id);

        IEnumerable<PetDto> breedPets = await _volunteersQueryRepository.GetPetsByBreedId(query, cancellationToken);

        if (breedPets?.Any() == true)
        {
            return Errors.General.ReferenceExist().ToErrorList();
        }

        breedForDelete.Delete();

        await _repository.SaveChangesAsync(specie, cancellationToken);

        _logger.LogInformation("Deleted breed {BreedName} with id {BreedId}",
            breedForDelete.Name, breedForDelete.Id);

        return (Guid)breedForDelete.Id;
    }
}
