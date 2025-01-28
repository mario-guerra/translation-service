using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudioTranslationService.Models.Service.Models
{

    public partial class AudioDownload
    {
        public string ContainerName { get; set; } = string.Empty;

        public string UploadId { get; set; } = string.Empty;

    }
}
