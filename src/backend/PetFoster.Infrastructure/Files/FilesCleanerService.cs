using Microsoft.Extensions.Logging;
using PetFoster.Application.Interfaces;
using PetFoster.Application.Messaging;

namespace PetFoster.Infrastructure.Files
{
    public class FilesCleanerService : IFilesCleanerService
    {
        private readonly IFileProvider _fileProvider;
        private readonly ILogger<FilesCleanerService> _logger;
        private readonly IMessageQueue<IEnumerable<Application.Files.FileInfo>> _messageQueue;

        public FilesCleanerService(
            IFileProvider fileProvider,
            ILogger<FilesCleanerService> logger,
            IMessageQueue<IEnumerable<Application.Files.FileInfo>> messageQueue)
        {
            _fileProvider = fileProvider;
            _logger = logger;
            _messageQueue = messageQueue;
        }

        public async Task Process(CancellationToken cancellationToken)
        {
            var fileInfos = await _messageQueue.ReadAsync(cancellationToken);

            foreach (var fileInfo in fileInfos)
            {
                await _fileProvider.RemoveFile(fileInfo, cancellationToken);
            }
        }
    }
}
