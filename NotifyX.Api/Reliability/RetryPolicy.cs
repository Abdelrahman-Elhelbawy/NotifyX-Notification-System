
namespace NotifyX.Api.Reliability;

public static class RetryPolicy
{
    private const int MaxDelaySeconds = 60;
    public static TimeSpan GetDelay(int attempts)
    {
        var seconds = Math.Min(
            Math.Pow(2, attempts),
            MaxDelaySeconds);

        var jitter = Random.Shared.NextDouble() * 0.2;

        var delaySeconds = Math.Min(
            seconds * (1 + jitter),
            MaxDelaySeconds);

        return TimeSpan.FromSeconds(delaySeconds);
    }
}
