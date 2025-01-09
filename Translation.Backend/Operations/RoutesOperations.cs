using System.Linq;
using System.Threading.Tasks;
using AudioTranslationService.Data;
using AudioTranslationService.Models.Service.Models;
using AudioTranslationService.Services;

namespace AudioTranslationService.Models.Service
{
    public class RoutesOperations : IRoutesOperations
    {
        private readonly EmailService _emailService;

        public RoutesOperations(EmailService emailService)
        {
            _emailService = emailService;
        }

        public Task<User> RegisterAsync(User user)
        {
            InMemoryStore.Users.Add(user);
            return Task.FromResult(user);
        }

        public Task<string> LoginAsync(User user)
        {
            var existingUser = InMemoryStore.Users
                .FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);

            if (existingUser != null)
            {
                return Task.FromResult("Login successful.");
            }

            return Task.FromResult("Invalid credentials.");
        }

        public Task<Payment> ProcessPaymentAsync(Payment payment)
        {
            InMemoryStore.Payments.Add(payment);
            return Task.FromResult(payment);
        }

        public Task<AudioUpload> UploadAudioAsync(AudioUpload audioUpload)
        {
            InMemoryStore.AudioUploads.Add(audioUpload);
            return Task.FromResult(audioUpload);
        }

        public Task<TranslationJob> StartTranslationAsync(TranslationJob translationJob)
        {
            translationJob.Status = "In Progress";
            InMemoryStore.TranslationJobs.Add(translationJob);
            return Task.FromResult(translationJob);
        }

        public Task<TranslationJob?> CheckStatusAsync(string jobId)
        {
            var job = InMemoryStore.TranslationJobs.FirstOrDefault(j => j.JobId == jobId);
            return Task.FromResult(job);
        }

        public async Task<byte[]> DownloadArtifactAsync(string jobId)
        {
            var job = InMemoryStore.TranslationJobs.FirstOrDefault(j => j.JobId == jobId);
            if (job != null && job.Status == "Completed")
            {
                // Simulate the artifact as a byte array
                var artifact = new byte[] { /* simulated data */ };

                // Send email notification
                var user = InMemoryStore.Users.FirstOrDefault(u => u.UserId == job.UploadId);
                if (user != null)
                {
                    var downloadLink = $"http://localhost:5000/download/{jobId}";
                    var emailBody = $"Your translation is ready. You can download it from the following link: {downloadLink}. The link is valid for 10 days.";
                    await _emailService.SendEmailAsync(user.Email, "Your Translation is Ready", emailBody);
                }

                return artifact;
            }

            return new byte[0];
        }
    }
}