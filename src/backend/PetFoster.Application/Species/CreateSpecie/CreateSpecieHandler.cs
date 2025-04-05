using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Application.Extensions;
using PetFoster.Application.Interfaces;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Species.CreateSpecie
{
    public sealed class CreateSpecieHandler : ICommandHandler<Guid, CreateSpecieCommand>
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
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid)
            {
                return validationResult.ToErrorList();
            }

            var specieName = SpecieName.Create(command.Name).Value;

            var exitSpecie = await _repository.GetByCriteriaAsync(s 
                => s.Name.Equals(specieName), cancellationToken);

            if (exitSpecie != null)
            {
                return Errors.Volunteer.AlreadyExist().ToErrorList();
            }

            var specieId = SpecieId.Create(command.Id);

            var newSpecie = new Specie(specieId, specieName);

            await _repository.AddAsync(newSpecie, cancellationToken);

            _logger.LogInformation("Created specie {SpecieName} with id {SpecieId}",
            newSpecie.Name.Value, newSpecie.Id.Value);

            return (Guid)newSpecie.Id;
        }
    }
}
