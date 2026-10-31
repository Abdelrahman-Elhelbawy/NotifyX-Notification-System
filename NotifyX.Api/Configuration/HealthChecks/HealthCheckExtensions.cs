using Microsoft.Extensions.Diagnostics.HealthChecks;
using NotifyX.Api.HealthChecks;
using static NotifyX.Api.HealthChecks.BackgroundWorkerHealthCheck;

namespace NotifyX.Api.Configuration.HealthChecks;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddNotificationHealthChecks(
        this IServiceCollection services)
    {
        services.AddSingleton<WorkerHeartbeat>();
        services.AddScoped<DatabaseHealthCheck>();
        services.AddScoped<BackgroundWorkerHealthCheck>();

        services.AddHealthChecks()
            .AddCheck(
                "self",
                () => HealthCheckResult.Healthy(),
                tags: new[] { "live" })
            .AddCheck<DatabaseHealthCheck>(
                "database",
                tags: new[] { "ready" })
            .AddCheck<BackgroundWorkerHealthCheck>(
                "background-worker",
                tags: new[] { "worker" });
                
        return services;
    }
}