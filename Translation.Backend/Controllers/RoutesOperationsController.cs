using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AudioTranslationService.Models.Service.Models;
using AudioTranslationService.Models.Service;
using AudioTranslationService.Services;
using AudioTranslationService.Models.Service.Controllers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AudioTranslationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoutesOperationsController : RoutesOperationsControllerBase
    {
        [HttpGet]
        [Route("/health")]
        public IActionResult Health()
        {
            return Ok(new { status = "Healthy", timestamp = System.DateTime.UtcNow });
        }
        private readonly EmailService _emailService;
        private readonly BlobStorageService _blobStorageService;
        private readonly CognitiveServicesClient _cognitiveServicesClient;
        private readonly ILogger<RoutesOperationsController> _logger;
        private readonly IServiceProvider _serviceProvider;

        public RoutesOperationsController(EmailService emailService, BlobStorageService blobStorageService, CognitiveServicesClient cognitiveServicesClient, ILogger<RoutesOperationsController> logger, ILogger<RoutesOperations> routesLogger, IServiceProvider serviceProvider)
        {
            _emailService = emailService;
            _blobStorageService = blobStorageService;
            _cognitiveServicesClient = cognitiveServicesClient;
            _logger = logger;
            _serviceProvider = serviceProvider;
            RoutesOperationsImpl = new RoutesOperations(_blobStorageService, _emailService, _cognitiveServicesClient, routesLogger, _serviceProvider);
        }

        internal override IRoutesOperations RoutesOperationsImpl { get; }

    }
}