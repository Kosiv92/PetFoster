using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Application.Extensions;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.GetPetsByBreedId;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;

namespace PetFoster.Application.Species.DeleteBreed
{
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
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid)
            {
                return validationResult.ToErrorList();
            }

            var specieId = SpecieId.Create(command.SpecieId);
            var breedId = BreedId.Create(command.BreedId);

            var specie = await _repository.GetByIdAsync(specieId, cancellationToken);

            if (specie == null)
                return Errors.General.NotFound(command.SpecieId).ToErrorList();    
            
            var breedForDelete = specie.Breeds.FirstOrDefault(b => b.Id == breedId);

            if (breedForDelete == null)
                return Errors.General.NotFound(command.BreedId).ToErrorList();

            var query = new GetPetsByBreedIdQuery(breedForDelete.Id);

            var breedPets = await _volunteersQueryRepository.GetPetsByBreedId(query, cancellationToken);

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
}
