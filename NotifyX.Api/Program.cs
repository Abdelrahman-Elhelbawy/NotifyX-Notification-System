// using Microsoft.AspNetCore.SignalR;
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
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        
        builder.Services.AddControllers();

        // Add dependencies to the container.
        builder.Services.NotifyXConfiguration(builderConfig);

        // builder.Services.AddSingleton<IUserIdProvider, QueryStringUserIdProvider>();

        var app = builder.Build();

        app.UseExceptionHandler();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseCors("NotifyXPolicy");

        app.UseHttpsRedirection();
        app.MapControllers();

        // Apply rate limiting to incoming requests.
        app.UseRateLimiter();

        // Map the notification hub for client real-time connections.
        app.MapHub<NotificationHub>("/hubs/notifications");


        // GET /health/live: checks API liveness using health checks tagged "live".
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live")
        });
        // GET /health/ready: checks whether the API is ready to serve requests using checks tagged "ready".
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });
        // GET /health/workers: checks background worker health using checks tagged "worker".
        app.MapHealthChecks("/health/workers", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("worker")
        });

        app.Run();
    }
}