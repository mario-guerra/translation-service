using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AudioTranslationService.Services
{
    public class ContainerCleanupService : BackgroundService
    {
        private readonly BlobStorageService _blobStorageService;
        private readonly ILogger<ContainerCleanupService> _logger;

        public ContainerCleanupService(BlobStorageService blobStorageService, ILogger<ContainerCleanupService> logger)
        {
            _blobStorageService = blobStorageService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Container cleanup service running at: {time}", DateTimeOffset.Now);

                await CleanupOldContainersAsync();

                // Wait for 24 hours before running the cleanup again
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task CleanupOldContainersAsync()
        {
            var containers = _blobStorageService.ListContainersAsync();
            await foreach (var container in containers)
            {
                var creationDate = await _blobStorageService.GetContainerCreationDateAsync(container.Name);
                if (creationDate.HasValue && (DateTime.UtcNow - creationDate.Value).TotalDays > 10)
                {
                    _logger.LogInformation("Deleting container {containerName} created on {creationDate}", container.Name, creationDate);
                    await _blobStorageService.DeleteContainerAsync(container.Name);
                }
            }
        }
    }
}