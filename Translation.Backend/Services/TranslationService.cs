using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using System;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AudioTranslationService.Models.Service.Models;
using AudioTranslationService.Services;

namespace AudioTranslationService.Services
{
    public class TranslationFunction
    {
        private readonly BlobStorageService _blobStorageService;
        private readonly CognitiveServicesClient _cognitiveServicesClient;
        private readonly EmailService _emailService;
        private readonly ILogger<TranslationFunction> _logger;

        public TranslationFunction(BlobStorageService blobStorageService, CognitiveServicesClient cognitiveServicesClient, EmailService emailService, ILogger<TranslationFunction> logger)
        {
            _blobStorageService = blobStorageService;
            _cognitiveServicesClient = cognitiveServicesClient;
            _emailService = emailService;
            _logger = logger;
        }

        [FunctionName("ProcessTranslation")]
        public async Task Run([QueueTrigger("translation-queue", Connection = "AzureWebJobsStorage")] string message)
        {
            var taskInfo = JsonSerializer.Deserialize<TranslationTaskInfo>(message);
            if (taskInfo == null)
            {
                _logger.LogError("Failed to deserialize the message to TranslationTaskInfo.");
                return;
            }

            _logger.LogInformation($"Processing translation for blob: {taskInfo.FileName} in container: {taskInfo.ContainerName}");

            try
            {
                // Download the audio file from Blob Storage
                var audioStream = await _blobStorageService.DownloadFileAsync(taskInfo.ContainerName, taskInfo.FileName);
                var localAudioFilePath = Path.Combine(Path.GetTempPath(), taskInfo.FileName);
                using (var fileStream = new FileStream(localAudioFilePath, FileMode.Create, FileAccess.Write))
                {
                    await audioStream.CopyToAsync(fileStream);
                }

                // Perform the translation using CognitiveServicesClient
                var translationResult = await _cognitiveServicesClient.TranslateAudioAsync(localAudioFilePath, taskInfo.LangIn, taskInfo.LangOut);

                // Save the transcription and translation to Blob Storage
                await SaveTranslationArtifacts(taskInfo.ContainerName, taskInfo.FileName, translationResult);

                // Send email notification with the download link
                await SendEmailNotification(taskInfo.UserId, taskInfo.ContainerName, taskInfo.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the translation.");
            }
        }

        private async Task SaveTranslationArtifacts(string containerName, string fileName, TranslationResult translationResult)
        {
            var uploadId = Path.GetFileNameWithoutExtension(fileName);

            // Save the transcription and translation as text files to Blob Storage
            await SaveToBlobStorageAsync(containerName, $"{uploadId}-transcription.txt", translationResult.Transcription);
            await SaveToBlobStorageAsync(containerName, $"{uploadId}-translation.txt", translationResult.Translation);

            // Optionally, synthesize audio in the target language and save to Blob Storage
            var synthesizedAudioFilePath = await SynthesizeAudioAsync(translationResult.Translation, containerName, uploadId);

            // Create and upload a zip file containing the transcription, translation, and synthesized audio (if exists)
            await CreateAndUploadZipFile(containerName, uploadId, synthesizedAudioFilePath);
        }

        private async Task SaveToBlobStorageAsync(string containerName, string blobName, string content)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                await _blobStorageService.UploadFileAsync(containerName, blobName, stream);
            }
        }

        private async Task<string?> SynthesizeAudioAsync(string text, string containerName, string uploadId)
        {
            var synthesizedAudioFilePath = Path.Combine(Path.GetTempPath(), $"{uploadId}-synthesized.wav");
            await _cognitiveServicesClient.SynthesizeAudioAsync(text, synthesizedAudioFilePath);

            // Upload the synthesized audio file to Blob Storage
            using (var synthesizedAudioStream = new FileStream(synthesizedAudioFilePath, FileMode.Open, FileAccess.Read))
            {
                await _blobStorageService.UploadFileAsync(containerName, $"{uploadId}-synthesized.wav", synthesizedAudioStream);
            }

            return synthesizedAudioFilePath;
        }

        private async Task CreateAndUploadZipFile(string containerName, string uploadId, string? synthesizedAudioFilePath)
        {
            var zipFilePath = Path.Combine(Path.GetTempPath(), $"{uploadId}-artifacts.zip");
            using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                zipArchive.CreateEntryFromFile(Path.Combine(Path.GetTempPath(), $"{uploadId}-transcription.txt"), "transcription.txt");
                zipArchive.CreateEntryFromFile(Path.Combine(Path.GetTempPath(), $"{uploadId}-translation.txt"), "translation.txt");

                if (synthesizedAudioFilePath != null)
                {
                    zipArchive.CreateEntryFromFile(synthesizedAudioFilePath, "translated-audio.wav");
                }
            }

            // Upload the zip file to Blob Storage
            using (var zipFileStream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read))
            {
                await _blobStorageService.UploadFileAsync(containerName, $"{uploadId}-artifacts.zip", zipFileStream);
            }
        }

        private async Task SendEmailNotification(string userId, string containerName, string uploadId)
        {
            var downloadLink = _emailService.GenerateDownloadLink(containerName, uploadId);
            var emailBody = $"<p>Your audio translation is complete. You can download the artifacts using the following link:</p><p><a href=\"{downloadLink}\">Download Translation Artifacts</a></p>";
            var payment = await GetFromBlobStorageAsync<Payment>(containerName, "payment.json");
            await _emailService.SendEmailAsync(payment.userEmail, "Your Audio Translation is Ready", emailBody);
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