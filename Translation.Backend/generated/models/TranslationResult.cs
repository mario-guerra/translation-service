using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudioTranslationService.Models.Service.Models
{
    public partial class TranslationResult
    {
        public string Transcription { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
    }
}