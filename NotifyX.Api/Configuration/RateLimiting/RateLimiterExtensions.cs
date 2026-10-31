using System.Security.Claims;
using System.Threading.RateLimiting;

namespace NotifyX.Api.Configuration.RateLimiting;

public static class RateLimiterExtensions
{
    public static IServiceCollection AddNotificationRateLimiting(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // 1. Token Bucket: Allow 10 requests initially, then refill 1 token every 6 seconds.
            options.AddPolicy(RateLimitPolicies.CreateNotification, context =>
                RateLimitPartition.GetTokenBucketLimiter(
                    GetPartitionKey(context),
                    _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 10,
                        TokensPerPeriod = 1,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(6),
                        AutoReplenishment = true,
                        QueueLimit = 0
                    }));

            // 2. Sliding Window: Allow 60 requests per minute.
            options.AddPolicy(RateLimitPolicies.ReadNotifications, context =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    GetPartitionKey(context),
                    _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 60,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 6,
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            // 3. Sliding Window: Allow 30 status requests per minute.
            options.AddPolicy(RateLimitPolicies.NotificationStatus, context =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    GetPartitionKey(context),
                    _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 30,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 6,
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            // 4. Fixed Window: Allow 20 updates per minute.
            options.AddPolicy(RateLimitPolicies.UpdateNotification, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    GetPartitionKey(context),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            // // 5. Concurrency: Allow only 3 simultaneous bulk requests per user.
            // options.AddPolicy(RateLimitPolicies.BulkCreation, context =>
            //     RateLimitPartition.GetConcurrencyLimiter(
            //         GetPartitionKey(context),
            //         _ => new ConcurrencyLimiterOptions
            //         {
            //             PermitLimit = 3,
            //             QueueLimit = 0,
            //             QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            //         }));

            // Handle rejected requests.
            options.OnRejected = async (context, cancellationToken) =>
            {
                var response = context.HttpContext.Response;

                response.StatusCode = StatusCodes.Status429TooManyRequests;

                response.ContentType = "application/json";

                await response.WriteAsJsonAsync(
                    new
                    {
                        status = 429,
                        error = "TooManyRequests",
                        message = "Too many requests. Please try again later."
                    },
                    cancellationToken);
            };
        });

        return services;
    }

    // Get a unique partition key for each authenticated user.
    // Fall back to the client's IP address for unauthenticated requests.
    private static string GetPartitionKey(HttpContext context)
    {
        var userId = context.User.FindFirst(
            ClaimTypes.NameIdentifier)?.Value
            ?? context.User.FindFirst("sub")?.Value;

        return userId
            ?? context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";
    }
}