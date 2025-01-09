using System.Linq;
using System.Threading.Tasks;
using AudioTranslationService.Data;
using AudioTranslationService.Models.Service.Models;
using AudioTranslationService.Models.Service.Controllers;

namespace AudioTranslationService.Models.Service
{
    public class OperationsOperations : IOperationsOperations
    {
        public Task<User?> GetUserProfileAsync()
        {
            // For simplicity, return the first user
            var user = InMemoryStore.Users.FirstOrDefault();
            return Task.FromResult(user);
        }

        public Task<string> UpdateUserProfileAsync(User user)
        {
            var existingUser = InMemoryStore.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (existingUser != null)
            {
                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.Password = user.Password;
            }
            else
            {
                InMemoryStore.Users.Add(user);
            }

            return Task.FromResult("User profile updated successfully.");
        }

        public Task<AudioUpload?> GetUploadDetailsAsync(string uploadId)
        {
            var upload = InMemoryStore.AudioUploads.FirstOrDefault(u => u.UploadId == uploadId);
            return Task.FromResult(upload);
        }

        public Task<string> DeleteUploadAsync(string uploadId)
        {
            var upload = InMemoryStore.AudioUploads.FirstOrDefault(u => u.UploadId == uploadId);
            if (upload != null)
            {
                InMemoryStore.AudioUploads.Remove(upload);
            }

            return Task.FromResult("Upload deleted successfully.");
        }

        public Task ListJobsAsync()
        {
            // For simplicity, just return the list of jobs
            var jobs = InMemoryStore.TranslationJobs;
            return Task.CompletedTask;
        }

        public Task<string> CancelJobAsync(string jobId)
        {
            var job = InMemoryStore.TranslationJobs.FirstOrDefault(j => j.JobId == jobId);
            if (job != null)
            {
                job.Status = "Cancelled";
            }

            return Task.FromResult("Job cancelled successfully.");
        }
    }
}