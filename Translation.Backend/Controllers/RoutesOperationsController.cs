using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AudioTranslationService.Models.Service.Models;
using AudioTranslationService.Models.Service;
using AudioTranslationService.Services;
using AudioTranslationService.Models.Service.Controllers;
using System.Threading.Tasks;

namespace AudioTranslationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoutesOperationsController : RoutesOperationsControllerBase
    {
        private readonly EmailService _emailService;
        private readonly BlobStorageService _blobStorageService;
        private readonly CognitiveServicesClient _cognitiveServicesClient;

        public RoutesOperationsController(EmailService emailService, BlobStorageService blobStorageService, CognitiveServicesClient cognitiveServicesClient)
        {
            _emailService = emailService;
            _blobStorageService = blobStorageService;
            _cognitiveServicesClient = cognitiveServicesClient;
            RoutesOperationsImpl = new RoutesOperations(_blobStorageService, _emailService, _cognitiveServicesClient);
        }

        internal override IRoutesOperations RoutesOperationsImpl { get; }

    }
}