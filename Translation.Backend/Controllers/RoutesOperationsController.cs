    [HttpGet]
    [Route("/health")]
    public IActionResult Health()
    {
        return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }
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

    // Improved error logging for payment and upload endpoints
    // Wrap actions in try/catch and log exceptions
    [HttpPost]
    [Route("/payment")]
    public override async Task<IActionResult> ProcessPayment([FromBody] Payment body)
    {
        try
        {
            var result = await RoutesOperationsImpl.ProcessPaymentAsync(body);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in /payment endpoint");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost]
    [Route("/upload")]
    public override async Task<IActionResult> UploadAudio([FromForm] IFormFile file, [FromForm] string userId, [FromForm] string langIn, [FromForm] string langOut)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            if (userId == null)
            {
                return BadRequest("No user ID provided.");
            }
            if (langIn == null)
            {
                return BadRequest("No input language provided.");
            }
            if (langOut == null)
            {
                return BadRequest("No output language provided.");
            }
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var audioUpload = new AudioUpload
                {
                    File = memoryStream.ToArray(),
                    LangIn = langIn,
                    LangOut = langOut,
                    UserId = userId
                };
                var result = await RoutesOperationsImpl.UploadAudioAsync(audioUpload);
                return Ok(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in /upload endpoint");
            return StatusCode(500, new { error = ex.Message });
        }
    }

        internal override IRoutesOperations RoutesOperationsImpl { get; }

    }
}