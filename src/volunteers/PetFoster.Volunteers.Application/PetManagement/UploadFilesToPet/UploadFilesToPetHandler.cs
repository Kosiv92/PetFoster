using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFoster.Core;
using PetFoster.Core.Abstractions;
using PetFoster.Core.DTO.Volunteer;
using PetFoster.Core.Extensions;
using PetFoster.Core.Messaging;
using PetFoster.SharedKernel;
using PetFoster.SharedKernel.ValueObjects;
using PetFoster.SharedKernel.ValueObjects.Ids;
using PetFoster.Volunteers.Application.Files;
using PetFoster.Volunteers.Application.Interfaces;
using PetFoster.Volunteers.Domain.Entities;

namespace PetFoster.Volunteers.Application.PetManagement.UploadFilesToPet;

public class UploadFilesToPetHandler : ICommandHandler<Guid, UploadFilesToPetCommand>
{
    private readonly IFileProvider _fileProvider;
    private readonly IRepository<Volunteer, VolunteerId> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UploadFilesToPetCommand> _validator;
    private readonly IMessageQueue<IEnumerable<Files.FileInfo>> _messageQueue;
    private readonly ILogger<UploadFilesToPetHandler> _logger;

    public UploadFilesToPetHandler(
        IFileProvider fileProvider,
        IRepository<Volunteer, VolunteerId> repository,
        [FromKeyedServices("volunteers")] IUnitOfWork unitOfWork,
        IValidator<UploadFilesToPetCommand> validator,
        IMessageQueue<IEnumerable<Files.FileInfo>> messageQueue,
        ILogger<UploadFilesToPetHandler> logger)
    {
        _fileProvider = fileProvider;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _messageQueue = messageQueue;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        UploadFilesToPetCommand command,
        CancellationToken cancellationToken = default)
    {
        FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
        {
            return validationResult.ToErrorList();
        }

        VolunteerId volunteerId = VolunteerId.Create(command.VolunteerId);

        Volunteer? volunteer = await _repository
        .GetByIdAsync(volunteerId, cancellationToken);

        if (volunteer == null)
        {
            return Errors.General.NotFound(volunteerId.Value).ToErrorList();
        }

        PetId petId = PetId.Create(command.PetId);

        Pet? pet = volunteer.FosteredAnimals.FirstOrDefault(a => a.Id == petId);
        if (pet == null)
        {
            return Errors.General.ValueIsInvalid($"Pet with id {command.PetId} not found in volunteer with id {command.VolunteerId}")
               .ToErrorList();
        }

        List<FileData> filesData = [];
        foreach (UploadFileDto file in command.Files)
        {
            string extension = Path.GetExtension(file.FileName);

            Result<FilePath, Error> filePath = FilePath.Create(Guid.NewGuid(), extension);
            if (filePath.IsFailure)
            {
                return filePath.Error.ToErrorList();
            }

            FileData fileData = new(file.Content, new Files.FileInfo(filePath.Value, Constants.FILES_BUCKET_NAME));

            filesData.Add(fileData);
        }

        Result<IReadOnlyList<FilePath>, Error> filePathsResult = await _fileProvider.UploadFiles(filesData, cancellationToken);
        if (filePathsResult.IsFailure)
        {
            await _messageQueue.WriteAsync(filesData.Select(f => f.FileInfo), cancellationToken);

            return filePathsResult.Error.ToErrorList();
        }

        List<PetFile> issueFiles = filePathsResult.Value
        .Select(f => new PetFile(f))
        .ToList();

        pet.UpdateFiles(issueFiles);

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("Success uploaded files to pet with id {PetId}", pet.Id.Value);

        return pet.Id.Value;
    }
}
