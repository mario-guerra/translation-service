using System.Linq;
using System.Threading.Tasks;
using AudioTranslationService.Data;
using AudioTranslationService.Models.Service.Models;
using AudioTranslationService.Models.Service.Controllers;


namespace AudioTranslationService.Models.Service
{
    public class NotificationsOperations : INotificationsOperations
    {
        public Task<string> ManageNotificationsAsync(NotificationPreferences notificationPreferences)
        {
            var existingPreferences = InMemoryStore.NotificationPreferences
                .FirstOrDefault(np => np.UserId == notificationPreferences.UserId);

            if (existingPreferences != null)
            {
                existingPreferences.EmailNotifications = notificationPreferences.EmailNotifications;
            }
            else
            {
                InMemoryStore.NotificationPreferences.Add(notificationPreferences);
            }

            return Task.FromResult("Notifications managed successfully.");
        }
    }
}