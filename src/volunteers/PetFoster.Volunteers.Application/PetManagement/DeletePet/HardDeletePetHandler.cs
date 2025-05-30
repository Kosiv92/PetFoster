﻿using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.Extensions;
using PetFoster.Core.Messaging;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Volunteers.Application.Interfaces;
using PetFoster.Volunteers.Domain.Entities;

namespace PetFoster.Volunteers.Application.PetManagement.DeletePet;

public sealed class HardDeletePetHandler : ICommandHandler<Guid, HardDeletePetCommand>
{

    private readonly IRepository<Volunteer, VolunteerId> _repository;
    private readonly IVolunteersQueryRepository _queryRepository;
    private readonly IValidator<HardDeletePetCommand> _validator;
    private readonly ILogger<DeletePetHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageQueue<IEnumerable<Files.FileInfo>> _messageQueue;

    public HardDeletePetHandler(IRepository<Volunteer, VolunteerId> repository,
        IVolunteersQueryRepository queryRepository,
        IValidator<HardDeletePetCommand> validator,
        ILogger<DeletePetHandler> logger,
        [FromKeyedServices("volunteers")] IUnitOfWork unitOfWork,
        IMessageQueue<IEnumerable<Files.FileInfo>> messageQueue)
    {
        _repository = repository;
        _queryRepository = queryRepository;
        _validator = validator;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _messageQueue = messageQueue;
    }

    public async Task<Result<Guid, ErrorList>> Handle(HardDeletePetCommand command,
        CancellationToken cancellationToken = default)
    {
        FluentValidation.Results.ValidationResult validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        System.Data.IDbTransaction transaction = await _unitOfWork.BeginTransaction();

        try
        {
            VolunteerId id = VolunteerId.Create(command.VolunteerId);

            Volunteer? volunteer = await _repository.GetByIdAsync(id, cancellationToken);

            if (volunteer == null)
            {
                return Errors.General.NotFound(command.VolunteerId).ToErrorList();
            }

            PetId petId = PetId.Create(command.PetId);

            Result<Pet, Error> result = volunteer.DeletePet(petId, true);

            if (result.IsFailure)
            {
                return Error.Failure("volunteer.delete.pet.failure", $"Failed to delete pet with id {result.Error}")
                    .ToErrorList();
            }


            if (result.Value.FileList.Any() == true)
            {
                List<Files.FileInfo> fileInfoList = [];

                foreach (PetFile file in result.Value.FileList)
                {
                    Files.FileInfo fileInfo = new(FilePath.Create(file.PathToStorage.Path).Value, Constants.FILES_BUCKET_NAME);
                    fileInfoList.Add(fileInfo);
                }

                await _messageQueue.WriteAsync(fileInfoList, cancellationToken);

                _logger.LogInformation("Successfully write files ({PetFilesCount}) to message queue of pet with id {PetId} in volunteer with id {VolunteerId}",
                    fileInfoList.Count, command.PetId, command.VolunteerId);
            }

            await _unitOfWork.SaveChanges(cancellationToken);

            transaction.Commit();

            _logger.LogInformation("Successfully delete (hard) pet with id {PetId} in volunteer with id {VolunteerId}",
                    command.PetId, command.VolunteerId);

            return command.PetId;
        }
        catch (Exception ex)
        {
            _logger.LogError("Throw exception {ExceptionMessage} while try delete (hard) pet with id {PetId} in volunteer with id {VolunteerId}. Transaction Rollback",
                command.PetId, command.VolunteerId, ex.Message);

            transaction.Rollback();

            return Error.Failure("volunteer.delete.pet.failure", $"Failed to delete pet with id {command.PetId}")
                .ToErrorList();
        }
    }
}
