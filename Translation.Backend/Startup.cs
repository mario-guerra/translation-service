using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AudioTranslationService.Models.Service;
using AudioTranslationService.Services;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSingleton<INotificationsOperations, NotificationsOperations>();
        services.AddSingleton<IOperationsOperations, OperationsOperations>();
        services.AddSingleton<IRoutesOperations, RoutesOperations>();
        services.AddSingleton<EmailService>();
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
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