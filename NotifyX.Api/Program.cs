
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using NotifyX.Api;
using NotifyX.Api.Hubs;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var builderConfig = builder.Configuration;

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddProblemDetails();

        builder.Services.AddOpenApi();

        // Register application dependencies and configurations.
        builder.Services.NotifyXConfiguration(builderConfig);

        var app = builder.Build();

        // Configure global exception handling.
        app.UseExceptionHandler();

        // Configure OpenAPI documentation.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // Configure the HTTP request pipeline.
        app.UseHttpsRedirection();

        app.UseCors("NotifyXPolicy");

        // Apply rate limiting to incoming requests.
        app.UseRateLimiter();

        // Enable authentication and authorization when configured.
        // app.UseAuthentication();
        // app.UseAuthorization();

        // Map API controllers.
        app.MapControllers();

        // Map the notification hub.
        app.MapHub<NotificationHub>("/hubs/notifications");

        // Check API liveness.
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live")
        });

        // Check API readiness.
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });

        // Check background workers health.
        app.MapHealthChecks("/health/workers", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("worker")
        });

        app.Run();
    }
}
