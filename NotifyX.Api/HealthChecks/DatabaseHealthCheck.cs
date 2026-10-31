using Microsoft.Extensions.Diagnostics.HealthChecks;
using NotifyX.Api.Data;

namespace NotifyX.Api.HealthChecks;

public sealed class DatabaseHealthCheck(ApplicationDbContext dbContext) : IHealthCheck
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync(
                cancellationToken);

            if (!canConnect)
            {
                return HealthCheckResult.Unhealthy(
                    "Cannot connect to the database.");
            }

            return HealthCheckResult.Healthy(
                "Database connection is healthy.");
        }
        catch (Exception)
        {
            return HealthCheckResult.Unhealthy(
                "Database connection failed.");
        }
    }
}