Below is a concise guide on how to perform speech synthesis (text-to-speech) in C# using the Azure Cognitive Services Speech SDK.

---

## 1. Prerequisites

1. Install the NuGet package:  
   Microsoft.CognitiveServices.Speech
2. Have a valid subscription key and service region for Azure Cognitive Services (e.g. "westus", "eastus").

---

## 2. Namespaces / Using Statements

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
```

---

## 3. SpeechSynthesisSamples Class

Several example methods are provided to demonstrate different ways of generating speech.

• Namespace: MicrosoftSpeechSDKSamples  
• Class Name: SpeechSynthesisSamples  

Each method is an async static Task, returning a Task to accommodate asynchronous operations.

---

### Method: SynthesisToSpeakerAsync
Synthesizes text directly to the default speaker.

Signature:
```csharp
public static async Task SynthesisToSpeakerAsync()
```

Steps:
1. Create a SpeechConfig from subscription key and region.
2. Create a SpeechSynthesizer with default audio output.
3. Repeatedly read text from the console, call synthesizer.SpeakTextAsync, and play the synthesized audio.

Input:  
• Subscription credentials  
• Text from console

Return:  
• Task (runs asynchronously)  

---

### Method: SynthesisWithLanguageAsync
Specifies a spoken language (e.g., "de-DE") and synthesizes text to the default speaker.

Signature:
```csharp
public static async Task SynthesisWithLanguageAsync()
```

Steps:
1. Create SpeechConfig with subscription key, set SpeechSynthesisLanguage.  
2. Create SpeechSynthesizer.  
3. Repeatedly read and synthesize text.

Input:  
• Subscription credentials  
• SpeechSynthesisLanguage

Return:  
• Task  

---

### Method: SynthesisWithVoiceAsync
Specifies a particular voice (e.g., "en-US, JennyNeural").

Signature:
```csharp
public static async Task SynthesisWithVoiceAsync()
```

Steps:
1. Create SpeechConfig and set SpeechSynthesisVoiceName.  
2. Create SpeechSynthesizer.  
3. Synthesize text and play it.

Input:  
• Subscription credentials  
• SpeechSynthesisVoiceName

Return:  
• Task  

---

### Method: SynthesisToWaveFileAsync
Saves synthesized speech to a WAV file.

Signature:
```csharp
public static async Task SynthesisToWaveFileAsync()
```

Steps:
1. Create SpeechConfig.  
2. Create file-based AudioConfig with FromWavFileOutput("outputaudio.wav").  
3. Create SpeechSynthesizer with that AudioConfig.  
4. Synthesize text, writing audio to the specified file.

Input:  
• Subscription credentials  
• WAV file path

Return:  
• Task  

---

### Method: SynthesisToMp3FileAsync
Saves synthesized speech to an MP3 file.

Signature:
```csharp
public static async Task SynthesisToMp3FileAsync()
```

Steps:
1. Create SpeechConfig and set output format (e.g., Audio16Khz32KBitRateMonoMp3).  
2. Use AudioConfig.FromWavFileOutput("outputaudio.mp3").  
3. Create SpeechSynthesizer and speak text.

Input:  
• Subscription credentials  
• MP3 file path

Return:  
• Task  

---

### Other Notable Methods
• SynthesisFileToMp3FileAsync: Synthesizes a large text file into one MP3 file.  
• SynthesisToPullAudioOutputStreamAsync: Streams synthesized audio to a pull stream.  
• SynthesisToPushAudioOutputStreamAsync: Feeds synthesized audio to a push stream.  
• SynthesisToResultAsync: Gets audio data directly from the result object (no file output).  
• SynthesisToAudioDataStreamAsync: Obtains an AudioDataStream for reading the synthesized audio.  
• SynthesisEventsAsync, SynthesisWordBoundaryEventAsync, SynthesisVisemeEventAsync, SynthesisBookmarkEventAsync: Demonstrate handling various synthesis events for advanced scenarios (visemes, bookmarks, word boundaries, etc.).

---

## 4. Basic Example Code

Below is a minimal example that synthesizes text to the default speaker:

```csharp


using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

public class SampleSynthesisProgram
{
    public static async Task Main()
    {
        // 1. Create config with your key and region
        var config = SpeechConfig.FromSubscription("YourSubscriptionKey", "YourServiceRegion");

        // 2. Use default speaker as output
        using var synthesizer = new SpeechSynthesizer(config);

        // 3. Synthesize
        Console.WriteLine("Enter text to speak:");
        var text = Console.ReadLine();

        using var result = await synthesizer.SpeakTextAsync(text);
        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            Console.WriteLine("Speech synthesized successfully.");
        }
        else if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
            Console.WriteLine($"Canceled: {cancellation.Reason}, Error={cancellation.ErrorDetails}");
        }
    }
}
```

---

## 5. Important Inputs & Return Values

• Subscription Key & Region: strings for your Azure resources.  
• Config Properties: SpeechSynthesisLanguage, SpeechSynthesisVoiceName, etc.  
• Methods Return: Task, with event-driven results indicating success or cancellation.  
• Event Handlers: Optional, for capturing partial data, word boundaries, etc.

---

With these methods, you can choose the output format (speaker, file, stream), language, voice, and more. Adapt the samples shown above to fit your environment, checking for final result status or cancellation details before concluding the speech synthesis.

**Plan**  
Below is a more complete C# example that shows how to use several of the Speech Synthesis methods in one place. It demonstrates setting up your subscription, choosing among different synthesis options (e.g., speaker, specified voice, language, saving to file), and handling results/cancellations.

---

```csharp


using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace SpeechSynthesisDemo
{
    public class Program
    {
        public static async Task Main()
        {
            // Provide your Azure subscription key and region here.
            var subscriptionKey = "YourSubscriptionKey";
            var serviceRegion = "YourServiceRegion";

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nSelect an option:\n" +
                                  "1) Synthesize to Default Speaker\n" +
                                  "2) Synthesize with Specific Language\n" +
                                  "3) Synthesize with Specific Voice\n" +
                                  "4) Synthesize to Wave File\n" +
                                  "5) Synthesize to MP3 File\n" +
                                  "0) Exit");
                Console.Write("> ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await SynthesisToSpeakerAsync(subscriptionKey, serviceRegion);
                        break;
                    case "2":
                        await SynthesisWithLanguageAsync(subscriptionKey, serviceRegion, "de-DE");
                        break;
                    case "3":
                        await SynthesisWithVoiceAsync(subscriptionKey, serviceRegion,
                            "Microsoft Server Speech Text to Speech Voice (en-US, JennyNeural)");
                        break;
                    case "4":
                        await SynthesisToWaveFileAsync(subscriptionKey, serviceRegion,
                            "outputaudio.wav");
                        break;
                    case "5":
                        await SynthesisToMp3FileAsync(subscriptionKey, serviceRegion,
                            "outputaudio.mp3");
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Unknown option.");
                        break;
                }
            }
        }

        // 1) Synthesize text to the default speaker.
        private static async Task SynthesisToSpeakerAsync(string subscriptionKey, string region)
        {
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);

            using var synthesizer = new SpeechSynthesizer(config);
            Console.WriteLine("Enter text to synthesize (empty to return).");
            while (true)
            {
                Console.Write("> ");
                string text = Console.ReadLine();
                if (string.IsNullOrEmpty(text))
                    break;

                using var result = await synthesizer.SpeakTextAsync(text);
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    Console.WriteLine($"Synthesized to speaker for text: \"{text}\"");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: {cancellation.Reason}, {cancellation.ErrorDetails}");
                }
            }
        }

        // 2) Synthesize text in a specified language (e.g., "de-DE").
        private static async Task SynthesisWithLanguageAsync(
            string subscriptionKey, string region, string language)
        {
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);
            config.SpeechSynthesisLanguage = language;

            using var synthesizer = new SpeechSynthesizer(config);
            Console.WriteLine($"Enter text to synthesize in language [{language}] (empty to return).");
            while (true)
            {
                Console.Write("> ");
                string text = Console.ReadLine();
                if (string.IsNullOrEmpty(text))
                    break;

                using var result = await synthesizer.SpeakTextAsync(text);
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    Console.WriteLine($"Synthesized in [{language}] for text: \"{text}\"");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: {cancellation.Reason}, {cancellation.ErrorDetails}");
                }
            }
        }

        // 3) Synthesize text using a specified voice (e.g., "en-US, JennyNeural").
        private static async Task SynthesisWithVoiceAsync(
            string subscriptionKey, string region, string voiceName)
        {
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);
            config.SpeechSynthesisVoiceName = voiceName;

            using var synthesizer = new SpeechSynthesizer(config);
            Console.WriteLine($"Enter text to synthesize with voice [{voiceName}] (empty to return).");
            while (true)
            {
                Console.Write("> ");
                string text = Console.ReadLine();
                if (string.IsNullOrEmpty(text))
                    break;

                using var result = await synthesizer.SpeakTextAsync(text);
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    Console.WriteLine($"Synthesized voice [{voiceName}] for text: \"{text}\"");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: {cancellation.Reason}, {cancellation.ErrorDetails}");
                }
            }
        }

        // 4) Synthesize text to a Wave file.
        private static async Task SynthesisToWaveFileAsync(
            string subscriptionKey, string region, string outputFile)
        {
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);

            using var fileOutput = AudioConfig.FromWavFileOutput(outputFile);
            using var synthesizer = new SpeechSynthesizer(config, fileOutput);
            Console.WriteLine($"Enter text to synthesize to a .wav file [{outputFile}] (empty to return).");
            while (true)
            {
                Console.Write("> ");
                string text = Console.ReadLine();
                if (string.IsNullOrEmpty(text))
                    break;

                using var result = await synthesizer.SpeakTextAsync(text);
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    Console.WriteLine($"Audio saved to [{outputFile}] for text: \"{text}\"");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: {cancellation.Reason}, {cancellation.ErrorDetails}");
                }
            }
        }

        // 5) Synthesize text to an MP3 file.
        private static async Task SynthesisToMp3FileAsync(
            string subscriptionKey, string region, string outputFile)
        {
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);
            // Configure MP3 output format
            config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio16Khz32KBitRateMonoMp3);

            using var fileOutput = AudioConfig.FromWavFileOutput(outputFile);
            using var synthesizer = new SpeechSynthesizer(config, fileOutput);
            Console.WriteLine($"Enter text to synthesize to an .mp3 file [{outputFile}] (empty to return).");
            while (true)
            {
                Console.Write("> ");
                string text = Console.ReadLine();
                if (string.IsNullOrEmpty(text))
                    break;

                using var result = await synthesizer.SpeakTextAsync(text);
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    Console.WriteLine($"MP3 saved to [{outputFile}] for text: \"{text}\"");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: {cancellation.Reason}, {cancellation.ErrorDetails}");
                }
            }
        }
    }
}
```

**Key Notes**  
• Replace "YourSubscriptionKey" and "YourServiceRegion" with valid Azure credentials.  
• Modify voice/language names to your desired settings.  
• When SynthesisToSpeakerAsync prompts you, type or paste text, then press Enter to hear it.  
• Press “0” to exit the menu.  

With this code, you have a clear workflow for each synthesis scenario, including speaker output, specific language, custom voices, and file output. Feel free to expand the logic (e.g., adding event handlers) for more granular control.