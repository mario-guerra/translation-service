using Azure;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using System;
using System.IO;
using System.Threading.Tasks;
using AudioTranslationService.Models.Service.Models;
using Microsoft.Extensions.Logging;

namespace AudioTranslationService.Services
{
    public class CognitiveServicesClient
    {
        private readonly SpeechConfig _speechConfig;
        private readonly string _translatorEndpoint;
        private readonly string _translatorApiKey;
        private readonly string _translatorRegion;
        private readonly Azure.AI.Translation.Text.TextTranslationClient _textTranslationClient;
        private readonly ILogger<CognitiveServicesClient> _logger;

        public CognitiveServicesClient(
            string speechSubscriptionKey,
            string speechRegion,
            string translatorEndpoint,
            string translatorApiKey,
            string translatorRegion,
            ILogger<CognitiveServicesClient> logger)
        {
            _logger = logger;
            var maskedKey = speechSubscriptionKey.Length > 4 ? new string('*', speechSubscriptionKey.Length - 4) + speechSubscriptionKey[^4..] : speechSubscriptionKey;
            _logger.LogInformation("Initializing CognitiveServicesClient with SpeechRegion: {SpeechRegion}, SpeechSubscriptionKey: {MaskedKey}", speechRegion, maskedKey);

            _speechConfig = SpeechConfig.FromSubscription(speechSubscriptionKey, speechRegion);
            _translatorEndpoint = translatorEndpoint;
            _translatorApiKey = translatorApiKey;
            _translatorRegion = translatorRegion;
            _textTranslationClient = new Azure.AI.Translation.Text.TextTranslationClient(
                new AzureKeyCredential(_translatorApiKey),
                new Uri(_translatorEndpoint),
                _translatorRegion
            );
        }

        public async Task<TranslationResult> TranslateAudioAsync(string audioFilePath, string fromLanguage, string targetLanguage)
        {
            // Step 1: Speech-to-text
            _logger.LogInformation("Speech-to-text: file={AudioFilePath}, language={FromLanguage}, region={Region}", audioFilePath, fromLanguage, _speechConfig.Region);
            var config = SpeechConfig.FromSubscription(_speechConfig.SubscriptionKey, _speechConfig.Region);
            config.SpeechRecognitionLanguage = fromLanguage;
            using var audioInput = AudioConfig.FromWavFileInput(audioFilePath);
            using var recognizer = new SpeechRecognizer(config, audioInput);

            var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);
            var transcription = result.Text;

            if (string.IsNullOrWhiteSpace(transcription))
            {
                _logger.LogError("Speech-to-text failed or returned empty transcription for file {AudioFilePath}", audioFilePath);
                throw new TranslationException("Speech-to-text failed or returned empty transcription.");
            }

            // Step 2: Text translation using dedicated Translator resource
            string translation = string.Empty;
            try
            {
                var response = _textTranslationClient.Translate(targetLanguage, transcription);
                var translations = response.Value;
                var translationItem = translations.FirstOrDefault();
                translation = translationItem?.Translations?.FirstOrDefault()?.Text ?? string.Empty;
            }
            catch (Azure.RequestFailedException exception)
            {
                _logger.LogError(exception, "Translation failed for text: {Transcription}", transcription);
                throw new TranslationException($"Translation failed: {exception.Message}", exception);
            }

            if (string.IsNullOrWhiteSpace(translation))
            {
                _logger.LogError("Translation returned empty result for text: {Transcription}", transcription);
                throw new TranslationException("Translation returned empty result.");
            }

            return new TranslationResult
            {
                Transcription = transcription,
                Translation = translation
            };
        }

        public async Task SynthesizeAudioAsync(string text, string outputFilePath)
        {
            if (string.IsNullOrWhiteSpace(_speechConfig.SubscriptionKey) || string.IsNullOrWhiteSpace(_speechConfig.Region))
            {
                throw new InvalidOperationException("SpeechConfig is missing subscription key or region. Please check your configuration.");
            }
            using var fileOutput = AudioConfig.FromWavFileOutput(outputFilePath);
            using var synthesizer = new SpeechSynthesizer(_speechConfig, fileOutput);

            var result = await synthesizer.SpeakTextAsync(text);
            if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                _logger.LogError("Speech synthesis canceled. Reason: {Reason}, ErrorDetails: {ErrorDetails}", cancellation.Reason, cancellation.ErrorDetails);
                throw new Exception($"Synthesis canceled: {cancellation.Reason}, {cancellation.ErrorDetails}");
            }
    }

}
public class TranslationException : Exception
{
    public TranslationException(string message) : base(message) { }
    public TranslationException(string message, Exception inner) : base(message, inner) { }
}
}
