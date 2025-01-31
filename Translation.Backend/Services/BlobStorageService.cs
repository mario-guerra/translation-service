using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Azure.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AudioTranslationService.Services
{
    /// <summary>
    /// Interact with Azure Blob Storage using identity-based access (i.e., DefaultAzureCredential).
    /// </summary>
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageService(string accountName)
        {
            // Use DefaultAzureCredential to authenticate to Azure
            var credential = new DefaultAzureCredential();
            _blobServiceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), credential);
            AccountName = accountName;
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
    }
}