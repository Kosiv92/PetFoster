using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFoster.Application.Database;
using PetFoster.Application.Extensions;
using PetFoster.Application.Files;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Messaging;
using PetFoster.Domain.Entities;
using PetFoster.Domain.Ids;
using PetFoster.Domain.Interfaces;
using PetFoster.Domain.Shared;
using PetFoster.Domain.ValueObjects;

namespace PetFoster.Application.Volunteers.UploadFilesToPet
{
    public class UploadFilesToPetHandler : ICommandHandler<Guid, UploadFilesToPetCommand>
    {
        private const string BUCKET_NAME = "files";

        private readonly IFileProvider _fileProvider;
        private readonly IRepository<Volunteer, VolunteerId> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UploadFilesToPetCommand> _validator;
        private readonly IMessageQueue<IEnumerable<Files.FileInfo>> _messageQueue;
        private readonly ILogger<UploadFilesToPetHandler> _logger;

        public UploadFilesToPetHandler(
            IFileProvider fileProvider,
            IRepository<Volunteer, VolunteerId> repository,
            IUnitOfWork unitOfWork,
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
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (validationResult.IsValid == false)
            {
                return validationResult.ToErrorList();
            }

            var volunteerId = VolunteerId.Create(command.VolunteerId);

            var volunteer = await _repository
            .GetByIdAsync(volunteerId, cancellationToken);

            if (volunteer == null)
                return Errors.General.NotFound(volunteerId.Value).ToErrorList();

            var petId = PetId.Create(command.PetId);

            var pet = volunteer.FosteredAnimals.FirstOrDefault(a => a.Id == petId);
            if (pet == null)
                return Errors.General.ValueIsInvalid($"Pet with id {command.PetId} not found in volunteer with id {command.VolunteerId}")
                   .ToErrorList();

            List<FileData> filesData = [];
            foreach (var file in command.Files)
            {
                var extension = Path.GetExtension(file.FileName);

                var filePath = FilePath.Create(Guid.NewGuid(), extension);
                if (filePath.IsFailure)
                    return filePath.Error.ToErrorList();

                var fileData = new FileData(file.Content, new Files.FileInfo(filePath.Value, BUCKET_NAME));

                filesData.Add(fileData);
            }

            var filePathsResult = await _fileProvider.UploadFiles(filesData, cancellationToken);
            if (filePathsResult.IsFailure)
            {
                await _messageQueue.WriteAsync(filesData.Select(f => f.FileInfo), cancellationToken);

                return filePathsResult.Error.ToErrorList();
            }

            var issueFiles = filePathsResult.Value
            .Select(f => new PetFile(f))
            .ToList();

            pet.UpdateFiles(issueFiles);

            await _unitOfWork.SaveChanges(cancellationToken);

            _logger.LogInformation("Success uploaded files to pet with id {PetId}", pet.Id.Value);

            return pet.Id.Value;
        }
    }
}
