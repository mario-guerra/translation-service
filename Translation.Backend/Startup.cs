using AudioTranslationService.Services;
using AudioTranslationService.Models.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        // Configure BlobStorageService
        services.AddSingleton(sp =>
        {
            var accountName = Configuration["BlobStorage:AccountName"] ?? throw new ArgumentNullException("BlobStorage:AccountName");
            return new BlobStorageService(accountName);
        });

        // Configure CognitiveServicesClient
        services.AddSingleton(sp =>
        {
            var speechSubscriptionKey = Configuration["CognitiveServices:SpeechSubscriptionKey"] ?? throw new ArgumentNullException("CognitiveServices:SpeechSubscriptionKey");
            var speechRegion = Configuration["CognitiveServices:SpeechRegion"] ?? throw new ArgumentNullException("CognitiveServices:SpeechRegion");
            return new CognitiveServicesClient(speechSubscriptionKey, speechRegion);
        });

        // Register EmailService with Azure Communication Services connection string
        services.AddSingleton(sp =>
        {
            var connectionString = Configuration["AzureCommunicationServices:ConnectionString"] ?? throw new ArgumentNullException("AzureCommunicationServices:ConnectionString");
            var senderAddress = Configuration["AzureCommunicationServices:MailFromAddress"] ?? throw new ArgumentNullException("AzureCommunicationServices:MailFromAddress");
            var baseUrl = Configuration["AzureCommunicationServices:BaseUrl"] ?? throw new ArgumentNullException("AzureCommunicationServices:BaseUrl");
            return new EmailService(connectionString, senderAddress, baseUrl);
        });

        // Register ContainerCleanupService as a hosted service
        services.AddHostedService<ContainerCleanupService>();

        // Register TranslationBackgroundService as a hosted service
        services.AddHostedService<TranslationBackgroundService>();

        // Register IOperationsOperations
        services.AddScoped<IOperationsOperations, OperationsOperations>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}