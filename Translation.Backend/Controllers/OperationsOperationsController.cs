using AudioTranslationService.Models.Service.Models;
using AudioTranslationService.Models.Service;
using AudioTranslationService.Models.Service.Controllers;
using AudioTranslationService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AudioTranslationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OperationsOperationsController : OperationsOperationsControllerBase
    {
        private readonly IOperationsOperations _operationsOperations;

        public OperationsOperationsController(BlobStorageService blobStorageService, EmailService emailService)
        {
            _operationsOperations = new OperationsOperations(blobStorageService, emailService);
        }

        internal override IOperationsOperations OperationsOperationsImpl => _operationsOperations;
    }
}