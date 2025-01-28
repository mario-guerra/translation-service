using System.CommandLine;
using System.CommandLine.Invocation;
using System.ClientModel;
using System.Net.Http;
using System.Net.Http.Headers;
using TranlsationService.Models;
using System.Text.Json;
using System.Text;
using System;
using System.IO;
using System.Threading.Tasks;
using TranlsationService;
using System.Linq;

namespace Translation.CLI
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var apiEndpointOption = new Option<string>("--api-endpoint", "The base URL for the API") { IsRequired = true };

            var rootCommand = new RootCommand("CLI for interacting with the Translation Service API");
            rootCommand.AddGlobalOption(apiEndpointOption);

            // Payment Command
            var paymentCommand = new Command("payment", "Process a payment");
            var userEmailOption = new Option<string>("--user-email", "Email of the user") { IsRequired = true };
            var amountOption = new Option<float>("--amount", "The payment amount") { IsRequired = true };
            var serviceOption = new Option<string>("--service", "The service being paid for") { IsRequired = true };
            var userIdOption = new Option<string>("--user-id", "The ID of the user") { IsRequired = true };
            var synthesizedAudioOption = new Option<bool>("--synthesized-audio", "Indicates if audio was synthesized") { IsRequired = true };

            paymentCommand.AddOption(userEmailOption);
            paymentCommand.AddOption(amountOption);
            paymentCommand.AddOption(serviceOption);
            paymentCommand.AddOption(userIdOption);
            paymentCommand.AddOption(synthesizedAudioOption);

            paymentCommand.SetHandler(async (InvocationContext context) =>
            {
                var apiEndpoint = context.ParseResult.GetValueForOption(apiEndpointOption);
                var userEmail = context.ParseResult.GetValueForOption(userEmailOption);
                var amount = context.ParseResult.GetValueForOption(amountOption);
                var service = context.ParseResult.GetValueForOption(serviceOption);
                var userId = context.ParseResult.GetValueForOption(userIdOption);
                var synthesizedAudio = context.ParseResult.GetValueForOption(synthesizedAudioOption);


                await PerformPaymentOperation(apiEndpoint!, userEmail!, amount, service!, userId!, synthesizedAudio);
            });


            // Upload Command
            var uploadCommand = new Command("upload", "Upload an audio file");
            var fileOption = new Option<FileInfo>("--file", "Path to the audio file") { IsRequired = true };
            var langInOption = new Option<string>("--lang-in", "The input language code") { IsRequired = true };
            var langOutOption = new Option<string>("--lang-out", "The output language code") { IsRequired = true };
            var uploadUserIdOption = new Option<string>("--user-id", "The ID of the user") { IsRequired = true }; // Added user ID option

            uploadCommand.AddOption(fileOption);
            uploadCommand.AddOption(langInOption);
            uploadCommand.AddOption(langOutOption);
            uploadCommand.AddOption(uploadUserIdOption); // Added user ID option

            uploadCommand.SetHandler(async (InvocationContext context) =>
            {
                var apiEndpoint = context.ParseResult.GetValueForOption(apiEndpointOption);
                var file = context.ParseResult.GetValueForOption(fileOption);
                var langIn = context.ParseResult.GetValueForOption(langInOption);
                var langOut = context.ParseResult.GetValueForOption(langOutOption);
                var uploadUserId = context.ParseResult.GetValueForOption(uploadUserIdOption); // Get user ID from option

                await PerformUploadOperation(apiEndpoint!, file!, langIn!, langOut!, uploadUserId!); // Pass user ID to method
            });

            // Download Command
            var downloadCommand = new Command("download", "Download an artifact");
            var containerNameOption = new Option<string>("--container-name", "The name of the container") { IsRequired = true };
            var uploadIdOption = new Option<string>("--upload-id", "The ID of the upload to download") { IsRequired = true };

            downloadCommand.AddOption(containerNameOption);
            downloadCommand.AddOption(uploadIdOption);

            downloadCommand.SetHandler(async (InvocationContext context) =>
            {
                var apiEndpoint = context.ParseResult.GetValueForOption(apiEndpointOption);
                var containerName = context.ParseResult.GetValueForOption(containerNameOption);
                var uploadId = context.ParseResult.GetValueForOption(uploadIdOption);

                await PerformDownloadOperation(apiEndpoint!, containerName!, uploadId!);
            });


            rootCommand.AddCommand(paymentCommand);
            rootCommand.AddCommand(uploadCommand);
            rootCommand.AddCommand(downloadCommand);


            return await rootCommand.InvokeAsync(args);
        }

        static async Task PerformPaymentOperation(string apiEndpoint, string userEmail, float amount, string service, string userId, bool synthesizedAudio)
        {
            Console.WriteLine($"Performing payment operation using API endpoint: {apiEndpoint}");

            try
            {
                var client = CreateApiClient(apiEndpoint!);
                var payment = TranlsationServiceModelFactory.Payment(userEmail!, amount, service!, userId!, synthesizedAudio);
                var result = await client.GetRoutesClient().ProcessPaymentAsync(payment);

                if (!result.GetRawResponse().IsError)
                {
                    Console.WriteLine("Payment processed successfully:");
                    Console.WriteLine($"  Message: {result.Value.Message}");
                    Console.WriteLine($"  User ID: {result.Value.UserId}");
                }
                else
                {
                    Console.WriteLine($"Error processing payment: {result.GetRawResponse().ReasonPhrase}");
                }

            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }


        static async Task PerformUploadOperation(string apiEndpoint, FileInfo file, string langIn, string langOut, string userId)
        {
            Console.WriteLine($"Performing upload operation using API endpoint: {apiEndpoint}");

            try
            {
                var client = CreateApiClient(apiEndpoint!);

                if (file == null || !file.Exists)
                {
                    Console.WriteLine($"Error: File not found at path {file?.FullName}");
                    return;
                }

                byte[] fileBytes = File.ReadAllBytes(file.FullName);

                var formData = new PublicMultiPartFormDataBinaryContent();
                formData.Add(fileBytes, "file", file.Name, "audio/wav");
                formData.Add(userId, "userId");
                formData.Add(langIn, "LangIn");
                formData.Add(langOut, "LangOut");

                // Add logging here
                Console.WriteLine($"Content-Type: {formData.ContentType}");
                //Console.WriteLine($"First 20 bytes of request body: {BitConverter.ToString(fileBytes.Take(20).ToArray()).Replace("-", "")}");

                var result = await client.GetRoutesClient().UploadAudioAsync(formData, formData.ContentType, null);

                if (!result.GetRawResponse().IsError)
                {
                    Console.WriteLine("Audio uploaded successfully:");
                    //Console.WriteLine($"  Message: {result.Value.Message}"); // This is not available on ClientResult
                    Console.WriteLine($"  Response: {result.GetRawResponse().Status}");
                }
                else
                {
                    Console.WriteLine($"Error uploading audio: {result.GetRawResponse().ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        static async Task PerformDownloadOperation(string apiEndpoint, string containerName, string uploadId)
        {
            Console.WriteLine($"Performing download operation using API endpoint: {apiEndpoint}");

            try
            {
                var client = CreateApiClient(apiEndpoint!);
                var result = await client.GetRoutesClient().DownloadArtifactAsync(containerName!, uploadId!);

                if (!result.GetRawResponse().IsError)
                {
                    Console.WriteLine("Artifact downloaded successfully:");
                    //  Console.WriteLine($"  Content: {result.Value}"); // This can be very long
                    //Optionally write out to a file
                    string fileName = $"{containerName}_{uploadId}.bin";
                    File.WriteAllBytes(fileName, result.GetRawResponse().Content.ToArray());
                    Console.WriteLine($"File saved to {fileName}");

                }
                else
                {
                    Console.WriteLine($"Error downloading artifact: {result.GetRawResponse().ReasonPhrase}");
                }

            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        static AudioTranslationServiceClient CreateApiClient(string apiEndpoint)
        {
            return new AudioTranslationServiceClient(new Uri(apiEndpoint));
        }

        static void HandleException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.ResetColor();
        }
    }
}