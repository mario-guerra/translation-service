using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AudioTranslationService.Models.Service.Models;
using AudioTranslationService.Services;
using Azure.Storage.Blobs;

namespace AudioTranslationService.Models.Service
{
    public class RoutesOperations : IRoutesOperations
    {
        private readonly BlobStorageService _blobStorageService;
        private readonly EmailService _emailService;
        private readonly CognitiveServicesClient _cognitiveServicesClient;

        public RoutesOperations(BlobStorageService blobStorageService, EmailService emailService, CognitiveServicesClient cognitiveServicesClient)
        {
            _blobStorageService = blobStorageService;
            _emailService = emailService;
            _cognitiveServicesClient = cognitiveServicesClient;
        }

        public async Task<SuccessResponse> UploadAudioAsync(AudioUpload audioUpload)
        {
            if (audioUpload.File == null || audioUpload.File.Length == 0)
            {
                throw new InvalidDataException("No file uploaded.");
            }

            if (audioUpload.UserId == null)
            {
                throw new InvalidDataException("No user ID provided.");
            }

            if (audioUpload.LangIn == null)
            {
                throw new InvalidDataException("No input language provided.");
            }

            if (audioUpload.LangOut == null)
            {
                throw new InvalidDataException("No output language provided.");
            }

            var userId = audioUpload.UserId;
            var containerName = $"user-{userId}";
            var uploadId = Guid.NewGuid().ToString();
            var fileName = $"{uploadId}.wav";

            using (var memoryStream = new MemoryStream(audioUpload.File))
            {
                await _blobStorageService.UploadFileAsync(containerName, fileName, memoryStream);
            }

            // Queue the translation task
            var translationTaskInfo = new TranslationTaskInfo
            {
                ContainerName = containerName,
                FileName = fileName,
                LangIn = audioUpload.LangIn,
                LangOut = audioUpload.LangOut,
                UserId = userId
            };
            await _blobStorageService.QueueTranslationTask(translationTaskInfo);

            return new SuccessResponse
            {
                Message = "Audio uploaded successfully. Translation will be processed shortly.",
                ContainerName = containerName,
                UploadId = uploadId
            };
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(Payment payment)
        {
            payment.userId = Guid.NewGuid().ToString();

            // Create a new blob storage container for the user
            var containerName = $"user-{payment.userId}";
            await _blobStorageService.CreateContainerAsync(containerName);

            // Store payment data in JSON format
            await SaveToBlobStorageAsync(containerName, "payment.json", payment);

            var email = payment.userEmail;
            // Send email receipt
            var emailBody = $"Thank you for your payment.\n\nUser ID: {payment.userId}\nService: {payment.Service}\nAmount: {payment.Amount}\n\nInvoice: {JsonSerializer.Serialize(payment)}";
            await _emailService.SendEmailAsync(email, "Payment Receipt", emailBody);

            return new PaymentResponse
            {
                Message = "Payment processed successfully.",
                UserId = payment.userId
            };
        }

        public async Task<byte[]> DownloadArtifactAsync(string containerName, string uploadId)
        {
            var zipFileName = $"{uploadId}-artifacts.zip";
            var stream = await _blobStorageService.GetZipFileFromBlobStorageAsync(containerName, zipFileName);
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private async Task SaveToBlobStorageAsync<T>(string containerName, string blobName, T data)
        {
            var jsonData = JsonSerializer.Serialize(data);
            using (var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonData)))
            {
                await _blobStorageService.UploadFileAsync(containerName, blobName, memoryStream);
            }
        }

        private async Task<T> GetFromBlobStorageAsync<T>(string containerName, string blobName)
        {
            var stream = await _blobStorageService.DownloadFileAsync(containerName, blobName);
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                var jsonData = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                return JsonSerializer.Deserialize<T>(jsonData)
                    ?? throw new InvalidOperationException($"Unable to deserialize JSON to {typeof(T).Name}.");
            }
        }
    }
}