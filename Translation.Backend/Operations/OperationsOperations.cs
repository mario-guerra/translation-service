using System.Linq;
using System.Threading.Tasks;
using AudioTranslationService.Models.Service.Models;
using AudioTranslationService.Models.Service.Controllers;
using AudioTranslationService.Services;

namespace AudioTranslationService.Models.Service
{
    public class OperationsOperations : IOperationsOperations
    {

        private readonly BlobStorageService _blobStorageService;
        private readonly EmailService _emailService;

        public OperationsOperations(BlobStorageService blobStorageService, EmailService emailService)
        {
            _blobStorageService = blobStorageService;
            _emailService = emailService;
        }

        public async Task<SuccessResponse> DeleteContainerAsync(ContainerName container)
        {
            try
            {
                await _blobStorageService.DeleteContainerAsync(container.Name);
                return new SuccessResponse
                {
                    Message = "Container deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new SuccessResponse
                {
                    Message = $"Failed to delete container: {ex.Message}"
                };
            }
        }
    }
}