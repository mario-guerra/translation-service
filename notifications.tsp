import "./models.tsp";
import "@typespec/http";
using TypeSpec.Http;
using AudioTranslationService.Models;

namespace AudioTranslationService.Notifications {
  @route("/notifications")
  @post
  op manageNotifications(@body notificationpreferences: NotificationPreferences): string;
}