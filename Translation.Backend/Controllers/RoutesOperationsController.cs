using Microsoft.AspNetCore.Mvc;
using AudioTranslationService.Models.Service.Models;
using AudioTranslationService.Models.Service;
using AudioTranslationService.Data;
using AudioTranslationService.Services;
using AudioTranslationService.Models.Service.Controllers;


namespace AudioTranslationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoutesOperationsController : RoutesOperationsControllerBase
    {
        private readonly EmailService _emailService;

        public RoutesOperationsController(EmailService emailService)
        {
            _emailService = emailService;
            RoutesOperationsImpl = new RoutesOperations(_emailService);
        }

        internal override IRoutesOperations RoutesOperationsImpl { get; }
    }
}