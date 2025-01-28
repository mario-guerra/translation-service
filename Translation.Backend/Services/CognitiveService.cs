using Azure;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using System;
using System.IO;
using System.Threading.Tasks;
using AudioTranslationService.Models.Service.Models;

namespace AudioTranslationService.Services
{
    public class CognitiveServicesClient
    {
        private readonly SpeechConfig _speechConfig;

        public CognitiveServicesClient(string speechSubscriptionKey, string speechRegion)
        {
            _speechConfig = SpeechConfig.FromSubscription(speechSubscriptionKey, speechRegion);
        }

        public async Task<TranslationResult> TranslateAudioAsync(string audioFilePath, string fromLanguage, string targetLanguage)
        {
            var config = SpeechTranslationConfig.FromSubscription(_speechConfig.SubscriptionKey, _speechConfig.Region);
            config.SpeechRecognitionLanguage = fromLanguage;
            config.AddTargetLanguage(targetLanguage);

            using var audioInput = AudioConfig.FromWavFileInput(audioFilePath);
            using var recognizer = new TranslationRecognizer(config, audioInput);

            var stopTranslation = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            var translationResult = new TranslationResult();
            var partialTranscription = string.Empty;
            var partialTranslation = string.Empty;

            // Subscribes to events.
            recognizer.Recognizing += (s, e) =>
            {
                Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
                foreach (var element in e.Result.Translations)
                {
                    Console.WriteLine($"    TRANSLATING into '{element.Key}': {element.Value}");
                }
            };

            recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.TranslatedSpeech)
                {
                    partialTranscription += e.Result.Text + " ";
                    foreach (var element in e.Result.Translations)
                    {
                        if (element.Key == targetLanguage)
                        {
                            partialTranslation += element.Value + " ";
                        }
                        Console.WriteLine($"    TRANSLATED into '{element.Key}': {element.Value}");
                    }
                }
                else if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                    Console.WriteLine($"    Speech not translated.");
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
            };

            recognizer.Canceled += (s, e) =>
            {
                Console.WriteLine($"CANCELED: Reason={e.Reason}");

                if (e.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }

                stopTranslation.TrySetResult(0);
            };

            recognizer.SessionStarted += (s, e) =>
            {
                Console.WriteLine("\nSession started event.");
            };

            recognizer.SessionStopped += (s, e) =>
            {
                Console.WriteLine("\nSession stopped event.");
                stopTranslation.TrySetResult(0);
            };

            // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
            Console.WriteLine("Start translation...");
            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

            // Waits for completion.
            await stopTranslation.Task;

            // Stops translation.
            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);

            translationResult.Transcription = partialTranscription.Trim();
            translationResult.Translation = partialTranslation.Trim();

            return translationResult;
        }

        public async Task SynthesizeAudioAsync(string text, string outputFilePath)
        {
            using var fileOutput = AudioConfig.FromWavFileOutput(outputFilePath);
            using var synthesizer = new SpeechSynthesizer(_speechConfig, fileOutput);

            var result = await synthesizer.SpeakTextAsync(text);
            if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                throw new Exception($"Synthesis canceled: {cancellation.Reason}, {cancellation.ErrorDetails}");
            }
        }
    }
}