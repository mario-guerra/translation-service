using System.Collections.Generic;
using AudioTranslationService.Models.Service.Models;

namespace AudioTranslationService.Data
{
    public static class InMemoryStore
    {
        public static List<User> Users { get; set; } = new List<User>();
        public static List<AudioUpload> AudioUploads { get; set; } = new List<AudioUpload>();
        public static List<TranslationJob> TranslationJobs { get; set; } = new List<TranslationJob>();
        public static List<NotificationPreferences> NotificationPreferences { get; set; } = new List<NotificationPreferences>();
        public static List<Payment> Payments { get; set; } = new List<Payment>();
    }
}