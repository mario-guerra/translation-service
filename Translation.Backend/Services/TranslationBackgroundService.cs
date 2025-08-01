using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Identity;
using System.Text;
using System.Text.Json;
using AudioTranslationService.Models.Service.Models;
using AudioTranslationService.Services;

namespace AudioTranslationService.Services
{
    public class TranslationBackgroundService : IHostedService, IAsyncDisposable
    {
        private const int MaxRetries = 2; // Number of retry attempts
        private readonly BlobStorageService _blobStorageService;
        private readonly CognitiveServicesClient _cognitiveServicesClient;
        private readonly EmailService _emailService;
        private readonly ILogger<TranslationBackgroundService> _logger;
        private readonly QueueClient _queueClient;
        private Task _backgroundTask;
        private CancellationTokenSource _cancellationTokenSource;

        public TranslationBackgroundService(
            BlobStorageService blobStorageService,
            CognitiveServicesClient cognitiveServicesClient,
            EmailService emailService,
            ILogger<TranslationBackgroundService> logger)
        {
            _blobStorageService = blobStorageService;
            _cognitiveServicesClient = cognitiveServicesClient;
            _emailService = emailService;
            _logger = logger;

            _backgroundTask = Task.CompletedTask;
            _cancellationTokenSource = new CancellationTokenSource();

            // Initialize the QueueClient using the correct storage account name
            var queueUri = new Uri($"https://{_blobStorageService.AccountName}.queue.core.windows.net/translation-queue");
            _queueClient = new QueueClient(queueUri, new DefaultAzureCredential());
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _backgroundTask = Task.Run(() => ProcessQueueMessagesAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_backgroundTask == null)
            {
                return;
            }

            _cancellationTokenSource.Cancel();

            await Task.WhenAny(_backgroundTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        public async ValueTask DisposeAsync()
        {
            _cancellationTokenSource?.Cancel();
            await Task.CompletedTask;
        }

        private async Task ProcessQueueMessagesAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checking queue for messages at: {time}", DateTimeOffset.Now);

                try
                {
                    var messages = await _queueClient.ReceiveMessagesAsync(maxMessages: 10, cancellationToken: stoppingToken);

                    foreach (var message in messages.Value)
                    {
                        _logger.LogInformation("Received message: {Message}", message.MessageText);

                        var taskInfo = JsonSerializer.Deserialize<TranslationTaskInfo>(message.MessageText);
                        if (taskInfo == null)
                        {
                            _logger.LogError("Failed to deserialize the message to TranslationTaskInfo.");
                            continue;
                        }

                        _logger.LogInformation("Deserialized TranslationTaskInfo: {TaskInfo}", taskInfo);

                        _logger.LogInformation($"Processing translation for blob: {taskInfo.FileName} in container: {taskInfo.ContainerName}");

                        string? localAudioFilePath = null;
                        string? transcriptionFilePath = null;
                        string? translationFilePath = null;
                        string? synthesizedAudioFilePath = null;
                        string? zipFilePath = null;

                        try
                        {
                            // Download the audio file from Blob Storage
                            _logger.LogInformation("Downloading audio file from Blob Storage: {ContainerName}/{FileName}", taskInfo.ContainerName, taskInfo.FileName);
                            var audioStream = await _blobStorageService.DownloadFileAsync(taskInfo.ContainerName, taskInfo.FileName);
                            localAudioFilePath = Path.Combine(Path.GetTempPath(), taskInfo.FileName);
                            using (var fileStream = new FileStream(localAudioFilePath, FileMode.Create, FileAccess.Write))
                            {
                                await audioStream.CopyToAsync(fileStream);
                            }
                            _logger.LogInformation("Downloaded audio file to: {LocalAudioFilePath}", localAudioFilePath);

                            // Perform the translation and other tasks with retry logic
                            for (int retryCount = 0; retryCount <= MaxRetries; retryCount++)
                            {
                                try
                                {
                                    (transcriptionFilePath, translationFilePath, synthesizedAudioFilePath, zipFilePath) = await SaveTranslationArtifacts(taskInfo.ContainerName, taskInfo.FileName, localAudioFilePath, taskInfo.LangIn, taskInfo.LangOut);
                                    break; // If successful, break out of the retry loop
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, $"Attempt {retryCount + 1} failed for translation artifacts. Message: {ex.Message}");
                                    if (retryCount == MaxRetries)
                                    {
                                        _logger.LogError(ex, $"Max retries reached for translation artifacts. Aborting processing.");
                                        throw; // Re-throw the exception to abort processing
                                    }
                                    _logger.LogWarning($"Retrying in 5 seconds...");
                                    await Task.Delay(5000); // Wait before retrying
                                }
                            }

                            // Send email notification with the download link
                            _logger.LogInformation("Sending email notification.");
                            await SendEmailNotification(taskInfo.UserId, taskInfo.ContainerName, Path.GetFileNameWithoutExtension(taskInfo.FileName));

                            // Delete the message from the queue
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "An error occurred while processing the translation.");
                            // Consider retrying the message or moving it to a dead-letter queue.
                        }
                        finally
                        {
                            // Cleanup temporary files
                            try
                            {
                                if (localAudioFilePath != null && File.Exists(localAudioFilePath)) File.Delete(localAudioFilePath);
                                if (transcriptionFilePath != null && File.Exists(transcriptionFilePath)) File.Delete(transcriptionFilePath);
                                if (translationFilePath != null && File.Exists(translationFilePath)) File.Delete(translationFilePath);
                                if (synthesizedAudioFilePath != null && File.Exists(synthesizedAudioFilePath)) File.Delete(synthesizedAudioFilePath);
                                if (zipFilePath != null && File.Exists(zipFilePath)) File.Delete(zipFilePath);
                            }
                            catch (Exception cleanupEx)
                            {
                                _logger.LogError(cleanupEx, "Error cleaning up temporary files.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing the queue.");
                }

                // Wait for a short interval before checking the queue again
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task<(string, string, string?, string?)> SaveTranslationArtifacts(string containerName, string fileName, string localAudioFilePath, string langIn, string langOut)
        {
            var uploadId = Path.GetFileNameWithoutExtension(fileName);
            string? transcriptionFilePath = null;
            string? translationFilePath = null;
            string? synthesizedAudioFilePath = null;
            string? zipFilePath = null;

            try
            {
                // Perform the translation using CognitiveServicesClient
                _logger.LogInformation("Performing translation using CognitiveServicesClient.");
                var translationResult = await _cognitiveServicesClient.TranslateAudioAsync(localAudioFilePath, langIn, langOut);
                _logger.LogInformation("Translation result: {TranslationResult}", translationResult);

                // Save the translation as a text file
                translationFilePath = Path.Combine(Path.GetTempPath(), $"{uploadId}-translation.txt");
                await File.WriteAllTextAsync(translationFilePath, translationResult.Translation);
                await SaveToBlobStorageAsync(containerName, $"{uploadId}-translation.txt", translationResult.Translation);

                // Create transcription file (using the original logic)
                transcriptionFilePath = Path.Combine(Path.GetTempPath(), $"{uploadId}-transcription.txt");
                // Assuming the translationResult also contains the transcription
                await File.WriteAllTextAsync(transcriptionFilePath, translationResult.Transcription ?? "No transcription available.");
                await SaveToBlobStorageAsync(containerName, $"{uploadId}-transcription.txt", translationResult.Transcription ?? "No transcription available.");

                // Synthesize audio
                synthesizedAudioFilePath = await SynthesizeAudioAsync(translationResult.Translation, containerName, uploadId);

                // Create and upload a zip file
                zipFilePath = await CreateAndUploadZipFile(containerName, uploadId, transcriptionFilePath, translationFilePath, synthesizedAudioFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving translation artifacts.");
                throw; // Re-throw to be caught in the main queue processing loop.
            }

            return (transcriptionFilePath, translationFilePath, synthesizedAudioFilePath, zipFilePath);
        }

        private async Task SaveToBlobStorageAsync(string containerName, string blobName, string content)
        {
            _logger.LogInformation("Uploading {BlobName} to container {ContainerName}", blobName, containerName);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                await _blobStorageService.UploadFileAsync(containerName, blobName, stream);
            }
            _logger.LogInformation("Uploaded {BlobName} to container {ContainerName}", blobName, containerName);
        }

        private async Task<string?> SynthesizeAudioAsync(string text, string containerName, string uploadId)
        {
            string? synthesizedAudioFilePath = null;
            try
            {
                synthesizedAudioFilePath = Path.Combine(Path.GetTempPath(), $"{uploadId}-synthesized.wav");
                _logger.LogInformation("Synthesizing audio to {SynthesizedAudioFilePath}", synthesizedAudioFilePath);
                await _cognitiveServicesClient.SynthesizeAudioAsync(text, synthesizedAudioFilePath);

                // Upload the synthesized audio file to Blob Storage
                _logger.LogInformation("Uploading synthesized audio to Blob Storage: {ContainerName}/{UploadId}-synthesized.wav", containerName, uploadId);
                using (var synthesizedAudioStream = new FileStream(synthesizedAudioFilePath, FileMode.Open, FileAccess.Read))
                {
                    await _blobStorageService.UploadFileAsync(containerName, $"{uploadId}-synthesized.wav", synthesizedAudioStream);
                }
                _logger.LogInformation("Uploaded synthesized audio to Blob Storage: {ContainerName}/{UploadId}-synthesized.wav", containerName, uploadId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error synthesizing audio.");
                return null; // Don't fail the entire process if audio synthesis fails.
            }

            return synthesizedAudioFilePath;
        }

        private async Task<string> CreateAndUploadZipFile(string containerName, string uploadId, string transcriptionFilePath, string translationFilePath, string? synthesizedAudioFilePath)
        {
            var zipFilePath = Path.Combine(Path.GetTempPath(), $"{uploadId}-artifacts.zip");
            _logger.LogInformation("Creating zip file at {ZipFilePath}", zipFilePath);
            try
            {
                using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                {
                    zipArchive.CreateEntryFromFile(transcriptionFilePath, "transcription.txt");
                    zipArchive.CreateEntryFromFile(translationFilePath, "translation.txt");

                    if (synthesizedAudioFilePath != null)
                    {
                        zipArchive.CreateEntryFromFile(synthesizedAudioFilePath, "translated-audio.wav");
                    }
                }

                // Upload the zip file to Blob Storage
                _logger.LogInformation("Uploading zip file to Blob Storage: {ContainerName}/{UploadId}-artifacts.zip", containerName, uploadId);
                using (var zipFileStream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read))
                {
                    await _blobStorageService.UploadFileAsync(containerName, $"{uploadId}-artifacts.zip", zipFileStream);
                }
                _logger.LogInformation("Uploaded zip file to Blob Storage: {ContainerName}/{UploadId}-artifacts.zip", containerName, uploadId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating and uploading zip file.");
                throw; // Re-throw to be caught in the main queue processing loop.
            }

            return zipFilePath;
        }

        private async Task SendEmailNotification(string userId, string containerName, string uploadId)
        {
            _logger.LogInformation("Generating download link for container {ContainerName} and uploadId {UploadId}", containerName, uploadId);
            var downloadLink = _emailService.GenerateDownloadLink(containerName, uploadId);
            var emailBody = $"<p>Your audio translation is complete. You can download the artifacts using the following link:</p><p><a href=\"{downloadLink}\">Download Translation Artifacts</a></p>";
            _logger.LogInformation("Fetching payment information from Blob Storage.");
            var payment = await GetFromBlobStorageAsync<Payment>(containerName, "payment.json");
            _logger.LogInformation("Sending email to {UserEmail}", payment.userEmail);
            await _emailService.SendEmailAsync(payment.userEmail, "Your Audio Translation is Ready", emailBody);
            _logger.LogInformation("Email sent to {UserEmail}", payment.userEmail);
        }

        private async Task<T> GetFromBlobStorageAsync<T>(string containerName, string blobName)
        {
            _logger.LogInformation("Downloading {BlobName} from container {ContainerName}", blobName, containerName);
            var stream = await _blobStorageService.DownloadFileAsync(containerName, blobName);
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                var jsonData = Encoding.UTF8.GetString(memoryStream.ToArray());
                _logger.LogInformation("Deserializing JSON data to {TypeName}", typeof(T).Name);
                return JsonSerializer.Deserialize<T>(jsonData)
                    ?? throw new InvalidOperationException($"Unable to deserialize JSON to {typeof(T).Name}.");
            }
        }
    }
}