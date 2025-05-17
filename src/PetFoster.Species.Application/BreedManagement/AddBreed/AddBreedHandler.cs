using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Database;
using PetFoster.Core.Extensions;
using PetFoster.Core.Interfaces;
using PetFoster.Core.ValueObjects;
using PetFoster.Species.Domain.Entities;
using PetFoster.Species.Domain.Ids;

namespace PetFoster.Species.Application.BreedManagement.AddBreed;

public sealed class AddBreedHandler : ICommandHandler<Guid, AddBreedCommand>
{
    private readonly ILogger<AddBreedHandler> _logger;
    private readonly IValidator<AddBreedCommand> _validator;
    private readonly IRepository<Specie, SpecieId> _specieRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddBreedHandler(ILogger<AddBreedHandler> logger, IValidator<AddBreedCommand> validator,
        IRepository<Specie, SpecieId> specieRepository, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _validator = validator;
        _specieRepository = specieRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> Handle(AddBreedCommand command,
        CancellationToken cancellationToken = default)
    {
        FluentValidation.Results.ValidationResult validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        SpecieId specieId = SpecieId.Create(command.SpecieId);

        Specie? specie = await _specieRepository.GetByIdAsync(specieId, cancellationToken);

        if (specie == null)
        {
            return Errors.General.NotFound(specieId.Value).ToErrorList();
        }

        System.Data.IDbTransaction transaction = await _unitOfWork.BeginTransaction();

        try
        {
            BreedName breedName = BreedName.Create(command.Name).Value;
            BreedId breedId = BreedId.NewBreedId();

            Breed breed = new(breedId, breedName);

            _ = specie.AddBreed(breed);

            await _unitOfWork.SaveChanges(cancellationToken);

            transaction.Commit();

            return breed.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError("Throw exception {ExceptionMessage} while try add breed to specie with id {SpecieId}. Transaction Rollback",
                ex.Message, specie.Id.Value);

            transaction.Rollback();

            return Error.Failure("specie.add.breed.failure", $"Failed to add breed to specie with id {specie.Id.Value}")
                .ToErrorList();
        }
    }
}
