using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NotifyX.Api.HealthChecks;

public sealed class BackgroundWorkerHealthCheck(BackgroundWorkerHealthCheck.WorkerHeartbeat heartbeat) : IHealthCheck
{
    private readonly WorkerHeartbeat _heartbeat = heartbeat;

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var lastHeartbeat = _heartbeat.LastHeartbeat;

        if (lastHeartbeat is null)
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy(
                    "Background worker has not started."));
        }

        var elapsed = DateTimeOffset.UtcNow - lastHeartbeat.Value;

        if (elapsed > TimeSpan.FromMinutes(2))
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy(
                    "Background worker heartbeat is stale."));
        }

        return Task.FromResult(
            HealthCheckResult.Healthy(
                "Background worker is running."));
    }

    public sealed class WorkerHeartbeat
    {
        private long _lastHeartbeatTicks;

        public DateTimeOffset? LastHeartbeat
        {
            get
            {
                var ticks = Interlocked.Read(ref _lastHeartbeatTicks);

                return ticks == 0
                    ? null
                    : new DateTimeOffset(ticks, TimeSpan.Zero);
            }
        }

        public void Beat()
        {
            Interlocked.Exchange(
                ref _lastHeartbeatTicks,
                DateTimeOffset.UtcNow.UtcTicks);
        }
    }
}