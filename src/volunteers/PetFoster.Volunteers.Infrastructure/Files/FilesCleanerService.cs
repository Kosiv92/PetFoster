using Microsoft.Extensions.Logging;
using PetFoster.Core.Messaging;
using PetFoster.Volunteers.Application.Interfaces;

namespace PetFoster.Volunteers.Infrastructure.Files;

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
        IEnumerable<Application.Files.FileInfo> fileInfos = await _messageQueue.ReadAsync(cancellationToken);

        foreach (Application.Files.FileInfo fileInfo in fileInfos)
        {
             await _fileProvider.RemoveFile(fileInfo, cancellationToken);
        }
    }
}
