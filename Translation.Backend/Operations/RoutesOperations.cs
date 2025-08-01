using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using AudioTranslationService.Models.Service.Models;
using AudioTranslationService.Services;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AudioTranslationService.Models.Service
{
    public class RoutesOperations : IRoutesOperations
    {
        private readonly BlobStorageService _blobStorageService;
        private readonly EmailService _emailService;
        private readonly CognitiveServicesClient _cognitiveServicesClient;
        private readonly ILogger<RoutesOperations> _logger;
        private readonly IServiceProvider _serviceProvider;

        public RoutesOperations(
            BlobStorageService blobStorageService,
            EmailService emailService,
            CognitiveServicesClient cognitiveServicesClient,
            ILogger<RoutesOperations> logger,
            IServiceProvider serviceProvider)
        {
            _blobStorageService = blobStorageService;
            _emailService = emailService;
            _cognitiveServicesClient = cognitiveServicesClient;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<SuccessResponse> UploadAudioAsync(AudioUpload audioUpload)
        {
            _logger.LogInformation("Starting UploadAudioAsync for UserId: {UserId}", audioUpload.UserId);

            if (audioUpload.File == null || audioUpload.File.Length == 0)
            {
                _logger.LogWarning("No file uploaded for UserId: {UserId}", audioUpload.UserId);
                throw new InvalidDataException("No file uploaded.");
            }

            if (audioUpload.UserId == null)
            {
                _logger.LogWarning("No user ID provided.");
                throw new InvalidDataException("No user ID provided.");
            }

            if (audioUpload.LangIn == null)
            {
                _logger.LogWarning("No input language provided for UserId: {UserId}", audioUpload.UserId);
                throw new InvalidDataException("No input language provided.");
            }

            if (audioUpload.LangOut == null)
            {
                _logger.LogWarning("No output language provided for UserId: {UserId}", audioUpload.UserId);
                throw new InvalidDataException("No output language provided.");
            }

            var userId = audioUpload.UserId;
            var containerName = $"user-{userId}";
            var uploadId = Guid.NewGuid().ToString();
            var fileName = $"{uploadId}.wav";

            _logger.LogInformation("Uploading file to Blob Storage for UserId: {UserId}, Container: {ContainerName}, FileName: {FileName}", userId, containerName, fileName);

            using (var memoryStream = new MemoryStream(audioUpload.File))
            {
                await _blobStorageService.UploadFileAsync(containerName, fileName, memoryStream);
            }

            _logger.LogInformation("File uploaded successfully for UserId: {UserId}, Container: {ContainerName}, FileName: {FileName}", userId, containerName, fileName);

            // Queue the translation task
            var translationTaskInfo = new TranslationTaskInfo
            {
                ContainerName = containerName,
                FileName = fileName,
                LangIn = audioUpload.LangIn,
                LangOut = audioUpload.LangOut,
                UserId = userId
            };

            _logger.LogInformation("Queueing translation task for UserId: {UserId}, Container: {ContainerName}, FileName: {FileName}", userId, containerName, fileName);
            await _blobStorageService.QueueTranslationTask(translationTaskInfo);

            _logger.LogInformation("Translation task queued successfully for UserId: {UserId}, Container: {ContainerName}, FileName: {FileName}", userId, containerName, fileName);

            return new SuccessResponse
            {
                Message = "Audio uploaded successfully. Translation will be processed shortly.",
                ContainerName = containerName,
                UploadId = uploadId
            };
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(Payment payment)
        {
            _logger.LogInformation("Starting ProcessPaymentAsync for UserEmail: {UserEmail}", payment.userEmail);

            // Validate required fields
            var missingFields = new List<string>();
            if (string.IsNullOrWhiteSpace(payment.userEmail)) missingFields.Add("userEmail");
            if (payment.Amount <= 0) missingFields.Add("amount");
            if (string.IsNullOrWhiteSpace(payment.Service)) missingFields.Add("service");

            if (missingFields.Count > 0)
            {
                _logger.LogWarning($"Payment request missing or invalid fields: {string.Join(", ", missingFields)}");
                throw new ArgumentException($"Missing or invalid required fields: {string.Join(", ", missingFields)}");
            }

            payment.userId = Guid.NewGuid().ToString();

            // Create a new blob storage container for the user
            var containerName = $"user-{payment.userId}";
            _logger.LogInformation("Creating container for UserId: {UserId}", payment.userId);
            await _blobStorageService.CreateContainerAsync(containerName);

            // Store payment data in JSON format
            _logger.LogInformation("Storing payment data for UserId: {UserId}", payment.userId);
            await SaveToBlobStorageAsync(containerName, "payment.json", payment);

            var email = payment.userEmail;
            // Send email receipt
            var emailBody = $"Thank you for your payment.\n\nUser ID: {payment.userId}\nService: {payment.Service}\nAmount: {payment.Amount}\n\nInvoice: {JsonSerializer.Serialize(payment)}";
            _logger.LogInformation("Sending email receipt to UserEmail: {UserEmail}", email);
            await _emailService.SendEmailAsync(email, "Payment Receipt", emailBody);

            _logger.LogInformation("Payment processed successfully for UserId: {UserId}", payment.userId);

            return new PaymentResponse
            {
                Message = "Payment processed successfully.",
                UserId = payment.userId
            };
        }

        public async Task<byte[]> DownloadArtifactAsync(string containerName, string uploadId)
        {
            _logger.LogInformation("Starting DownloadArtifactAsync for Container: {ContainerName}, UploadId: {UploadId}", containerName, uploadId);

            var zipFileName = $"{uploadId}-artifacts.zip";
            var stream = await _blobStorageService.GetZipFileFromBlobStorageAsync(containerName, zipFileName);

            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                _logger.LogInformation("Artifact downloaded successfully for Container: {ContainerName}, UploadId: {UploadId}", containerName, uploadId);
                return memoryStream.ToArray();
            }
        }

        private async Task SaveToBlobStorageAsync<T>(string containerName, string blobName, T data)
        {
            _logger.LogInformation("Saving data to Blob Storage for Container: {ContainerName}, BlobName: {BlobName}", containerName, blobName);

            var jsonData = JsonSerializer.Serialize(data);
            using (var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonData)))
            {
                await _blobStorageService.UploadFileAsync(containerName, blobName, memoryStream);
            }

            _logger.LogInformation("Data saved to Blob Storage for Container: {ContainerName}, BlobName: {BlobName}", containerName, blobName);
        }

        private async Task<T> GetFromBlobStorageAsync<T>(string containerName, string blobName)
        {
            _logger.LogInformation("Getting data from Blob Storage for Container: {ContainerName}, BlobName: {BlobName}", containerName, blobName);

            var stream = await _blobStorageService.DownloadFileAsync(containerName, blobName);

            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                var jsonData = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                _logger.LogInformation("Data retrieved from Blob Storage for Container: {ContainerName}, BlobName: {BlobName}", containerName, blobName);
                return JsonSerializer.Deserialize<T>(jsonData)
                    ?? throw new InvalidOperationException($"Unable to deserialize JSON to {typeof(T).Name}.");
            }
        }
    }
}