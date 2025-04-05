using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Application.Extensions;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Volunteers.GetPetsBySpecieId;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;

namespace PetFoster.Application.Species.DeleteSpecie
{
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
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid)
            {
                return validationResult.ToErrorList();
            }

            var id = SpecieId.Create(command.Id);

            var specieForDelete = await _repository.GetByIdAsync(id, cancellationToken);

            if (specieForDelete == null)
                return Errors.General.NotFound(command.Id).ToErrorList();

            var query = new GetPetsBySpecieIdQuery(specieForDelete.Id);

            var speciesPets = await _volunteersQueryRepository.GetPetsBySpecieId(query, cancellationToken);

            if(speciesPets?.Any() == true)
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
}
