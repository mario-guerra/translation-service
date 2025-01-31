using System;
using System.IO;
using System.IO.Compression;
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
            var inLanguage = audioUpload.LangIn;
            var outLanguage = audioUpload.LangOut;
            var uploadId = Guid.NewGuid().ToString();
            var fileName = $"{uploadId}.wav";

            using (var memoryStream = new MemoryStream(audioUpload.File))
            {
                await _blobStorageService.UploadFileAsync(containerName, fileName, memoryStream);
            }

            // Retrieve payment information to check if synthesized audio is required
            var payment = await GetFromBlobStorageAsync<Payment>(containerName, "payment.json");

            // Download the audio file from Blob Storage to a local path
            var localAudioFilePath = Path.Combine(Path.GetTempPath(), fileName);
            var audioStream = await _blobStorageService.DownloadFileAsync(containerName, fileName);
            using (var fileStream = new FileStream(localAudioFilePath, FileMode.Create, FileAccess.Write))
            {
                await audioStream.CopyToAsync(fileStream);
            }

            var translationResult = await _cognitiveServicesClient.TranslateAudioAsync(localAudioFilePath, inLanguage, outLanguage);

            // Save the transcription and translation as text files locally
            var transcriptionFileName = Path.Combine(Path.GetTempPath(), $"{uploadId}-transcription.txt");
            var translationFileName = Path.Combine(Path.GetTempPath(), $"{uploadId}-translation.txt");
            await File.WriteAllTextAsync(transcriptionFileName, translationResult.Transcription);
            await File.WriteAllTextAsync(translationFileName, translationResult.Translation);

            // Save the transcription and translation as text files to Blob Storage
            await SaveToBlobStorageAsync(containerName, $"{uploadId}-transcription.txt", translationResult.Transcription);
            await SaveToBlobStorageAsync(containerName, $"{uploadId}-translation.txt", translationResult.Translation);

            string? synthesizedAudioFilePath = null;
            if (payment.SynthesizedAudio)
            {
                // Synthesize audio in the target language
                synthesizedAudioFilePath = Path.Combine(Path.GetTempPath(), $"{uploadId}-synthesized.wav");
                await _cognitiveServicesClient.SynthesizeAudioAsync(translationResult.Translation, synthesizedAudioFilePath);

                // Upload the synthesized audio file to Blob Storage
                using (var synthesizedAudioStream = new FileStream(synthesizedAudioFilePath, FileMode.Open, FileAccess.Read))
                {
                    await _blobStorageService.UploadFileAsync(containerName, $"{uploadId}-synthesized.wav", synthesizedAudioStream);
                }
            }

            // Create a zip file containing the transcription, translation, and synthesized audio (if exists)
            var zipFilePath = Path.Combine(Path.GetTempPath(), $"{uploadId}-artifacts.zip");
            using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                zipArchive.CreateEntryFromFile(transcriptionFileName, $"transcription.txt");
                zipArchive.CreateEntryFromFile(translationFileName, $"translation.txt");

                if (synthesizedAudioFilePath != null)
                {
                    zipArchive.CreateEntryFromFile(synthesizedAudioFilePath, $"translated-audio.wav");
                }
            }

            // Upload the zip file to Blob Storage
            using (var zipFileStream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read))
            {
                await _blobStorageService.UploadFileAsync(containerName, $"{uploadId}-artifacts.zip", zipFileStream);
            }

            // Generate the download link
            var downloadLink = _emailService.GenerateDownloadLink(containerName, uploadId);

            // Send email notification with the download link
            var emailBody = $"<p>Your audio translation is complete. You can download the artifacts using the following link:</p><p><a href=\"{downloadLink}\">Download Translation Artifacts</a></p>";
            await _emailService.SendEmailAsync(payment.userEmail, "Your Audio Translation is Ready", emailBody);

            return new SuccessResponse
            {
                Message = "Audio uploaded and translation completed successfully. Check your email for a download link.",
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