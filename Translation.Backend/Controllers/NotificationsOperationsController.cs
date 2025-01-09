using Microsoft.AspNetCore.Mvc;
using AudioTranslationService.Models.Service.Models;
using AudioTranslationService.Models.Service;
using AudioTranslationService.Data;
using AudioTranslationService.Models.Service.Controllers;

namespace AudioTranslationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationsOperationsController : NotificationsOperationsControllerBase
    {
        internal override INotificationsOperations NotificationsOperationsImpl { get; } = new NotificationsOperations();
    }
}