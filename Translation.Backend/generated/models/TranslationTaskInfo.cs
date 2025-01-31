using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudioTranslationService.Models.Service.Models
{
    public partial class TranslationTaskInfo
    {
        public required string ContainerName { get; set; }
        public required string FileName { get; set; }
        public required string LangIn { get; set; }
        public required string LangOut { get; set; }
        public required string UserId { get; set; }
    }
}