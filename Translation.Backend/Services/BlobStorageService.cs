using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Azure.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core.Diagnostics;
using AudioTranslationService.Models.Service.Models;

namespace AudioTranslationService.Services
{
    /// <summary>
    /// Interact with Azure Blob Storage using identity-based access (i.e., DefaultAzureCredential).
    /// </summary>
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly QueueServiceClient _queueServiceClient;

        public BlobStorageService(string accountName)
        {
            try
            {
                // Use DefaultAzureCredential to authenticate to Azure
                var userAssignedClientId = "40c739d7-20a7-4333-a7dc-4675ca1fc9ad";
                //var options = new DefaultCredentialOptions { TenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47" };
                using AzureEventSourceListener listener = AzureEventSourceListener.CreateConsoleLogger();
                var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = userAssignedClientId, TenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47" });
                //var credential = new AzureCliCredential(options);
                _blobServiceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), credential);
                _queueServiceClient = new QueueServiceClient(new Uri($"https://{accountName}.queue.core.windows.net"), credential);
                AccountName = accountName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing BlobStorageService: {ex.Message}");
                throw;
            }
        }

        public string AccountName { get; }

        /// <summary>
        /// Creates a container if it does not already exist. Also sets metadata indicating creation date.
        /// </summary>
        public async Task CreateContainerAsync(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            var metadata = new Dictionary<string, string>
            {
                { "CreationDate", DateTime.UtcNow.ToString("o") }
            };
            await containerClient.SetMetadataAsync(metadata);
        }

        /// <summary>
        /// Uploads a file to the specified container and blob name.
        /// </summary>
        public async Task UploadFileAsync(string containerName, string blobName, Stream fileStream)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(fileStream, overwrite: true);
        }

        /// <summary>
        /// Downloads a file from the specified container and blob name.
        /// </summary>
        public async Task<Stream> DownloadFileAsync(string containerName, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            var downloadInfo = await blobClient.DownloadAsync();
            return downloadInfo.Value.Content;
        }

        /// <summary>
        /// Deletes a container if it exists.
        /// </summary>
        public async Task DeleteContainerAsync(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Retrieves the container creation date if available in its metadata.
        /// </summary>
        public async Task<DateTime?> GetContainerCreationDateAsync(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var properties = await containerClient.GetPropertiesAsync();
            if (properties.Value.Metadata.TryGetValue("CreationDate", out var creationDate))
            {
                return DateTime.Parse(creationDate);
            }
            return null;
        }

        /// <summary>
        /// Lists all containers in this storage account.
        /// </summary>
        public async IAsyncEnumerable<BlobContainerItem> ListContainersAsync()
        {
            await foreach (var container in _blobServiceClient.GetBlobContainersAsync())
            {
                yield return container;
            }
        }

        /// <summary>
        /// Retrieves a .zip file from the specified container if it exists, returning the file as a stream.
        /// </summary>
        public async Task<Stream> GetZipFileFromBlobStorageAsync(string containerName, string zipFileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(zipFileName);

            if (await blobClient.ExistsAsync())
            {
                var downloadInfo = await blobClient.DownloadAsync();
                return downloadInfo.Value.Content;
            }
            else
            {
                throw new FileNotFoundException($"The file '{zipFileName}' was not found in the container '{containerName}'.");
            }
        }

        /// <summary>
        /// Queues a translation task.
        /// </summary>
        public async Task QueueTranslationTask(TranslationTaskInfo taskInfo)
        {
            var queueClient = _queueServiceClient.GetQueueClient("translation-queue");
            await queueClient.CreateIfNotExistsAsync();

            var message = JsonSerializer.Serialize(taskInfo);
            await queueClient.SendMessageAsync(message);
        }
    }
}