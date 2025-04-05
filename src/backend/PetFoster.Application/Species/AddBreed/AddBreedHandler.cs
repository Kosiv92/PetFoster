using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Application.Database;
using PetFoster.Application.Extensions;
using PetFoster.Application.Interfaces;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Species.AddBreed
{
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
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid)
            {
                return validationResult.ToErrorList();
            }

            SpecieId specieId = SpecieId.Create(command.SpecieId);

            var specie = await _specieRepository.GetByIdAsync(specieId, cancellationToken);

            if (specie == null)
            {
                return Errors.General.NotFound(specieId.Value).ToErrorList();
            }

            var transaction = await _unitOfWork.BeginTransaction();

            try
            {
                var breedName = BreedName.Create(command.Name).Value;
                var breedId = BreedId.NewBreedId();

                var breed = new Breed(breedId, breedName);

                specie.AddBreed(breed);

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
}
