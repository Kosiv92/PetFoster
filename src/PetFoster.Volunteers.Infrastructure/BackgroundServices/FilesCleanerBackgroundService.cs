using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PetFoster.Application.Interfaces;

namespace PetFoster.Volunteers.Infrastructure.BackgroundServices
{
    public class FilesCleanerBackgroundService : BackgroundService
    {
        private readonly ILogger<FilesCleanerBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public FilesCleanerBackgroundService(
        ILogger<FilesCleanerBackgroundService> logger,
        IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Files cleaner background service is starting");

            await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();

            IFilesCleanerService filesCleanerService = scope.ServiceProvider.GetRequiredService<IFilesCleanerService>();

            while (!cancellationToken.IsCancellationRequested)
            {
                await filesCleanerService.Process(cancellationToken);
            }
            await Task.CompletedTask;
        }
    }
}
