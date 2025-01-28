using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AudioTranslationService.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly StorageSharedKeyCredential _sharedKeyCredential;

        public BlobStorageService(string accountName, string accountKey)
        {
            _sharedKeyCredential = new StorageSharedKeyCredential(accountName, accountKey);
            _blobServiceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), _sharedKeyCredential);
            AccountName = accountName;
        }

        public string AccountName { get; }

        public async Task CreateContainerAsync(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            // Store the creation date as metadata
            var metadata = new Dictionary<string, string>
            {
                { "CreationDate", DateTime.UtcNow.ToString("o") }
            };
            await containerClient.SetMetadataAsync(metadata);
        }

        public async Task UploadFileAsync(string containerName, string blobName, Stream fileStream)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(fileStream, overwrite: true);
        }

        public async Task<Stream> DownloadFileAsync(string containerName, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            var downloadInfo = await blobClient.DownloadAsync();
            return downloadInfo.Value.Content;
        }

        public async Task DeleteContainerAsync(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.DeleteIfExistsAsync();
        }

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

        public async IAsyncEnumerable<BlobContainerItem> ListContainersAsync()
        {
            await foreach (var container in _blobServiceClient.GetBlobContainersAsync())
            {
                yield return container;
            }
        }

        public Uri GenerateUploadSasToken(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                Resource = "c", // "c" for container
                ExpiresOn = DateTimeOffset.UtcNow.AddDays(10) // Set the expiry time to 10 days
            };

            sasBuilder.SetPermissions(BlobContainerSasPermissions.Write | BlobContainerSasPermissions.Create);

            var sasToken = sasBuilder.ToSasQueryParameters(_sharedKeyCredential).ToString();

            return new Uri($"{containerClient.Uri}?{sasToken}");
        }

        public Uri GenerateReadOnlySasToken(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                Resource = "c", // "c" for container
                ExpiresOn = DateTimeOffset.UtcNow.AddDays(10) // Set the expiry time to 10 days
            };

            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

            var sasToken = sasBuilder.ToSasQueryParameters(_sharedKeyCredential).ToString();

            return new Uri($"{containerClient.Uri}?{sasToken}");
        }

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