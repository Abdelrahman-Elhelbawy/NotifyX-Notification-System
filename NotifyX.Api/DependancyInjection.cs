using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using NotifyX.Api.BackgroundServices;
using NotifyX.Api.Common.Exceptions;
using NotifyX.Api.Configuration.ApiVersioning;
using NotifyX.Api.Configuration.HealthChecks;
using NotifyX.Api.Configuration.RateLimiting;
using NotifyX.Api.Data;
using NotifyX.Api.Factories;
using NotifyX.Api.Publishers;
using NotifyX.Api.Repositories.Notifications;
using NotifyX.Api.Repository.IdempotencyKeys;
using NotifyX.Api.Repository.IdempotencyKeysRepository;
using NotifyX.Api.Repository.Notifcations;
using NotifyX.Api.Repository.NotificationRecipients;
using NotifyX.Api.Services.Cache;
using NotifyX.Api.Services.Idempotency;
using NotifyX.Api.Services.NotificationRecipients;
using NotifyX.Api.Services.Notifications;
using NotifyX.Api.Services.Outbox;

namespace NotifyX.Api;

public static class DependencyInjection
{
    /// <summary>
    /// Configures the NotifyX API dependencies in the service collection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Application configuration used to register database and infrastructure services.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, configured with NotifyX dependencies.</returns>
    public static IServiceCollection NotifyXConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
        // Apply rate limiting policies to protect API endpoints from excessive requests.
        .AddNotificationRateLimiting()
        // Register health checks to monitor API, database, and background worker availability.
        .AddNotificationHealthChecks()
        // Register Versioning to monitor API V1. V2. V3.
        .AddApiVersioningConfiguration()
        // Register scoped application services and their dependencies.
        .AddScopedConfiguration()
        // Register FluentValidation validators and configure automatic request validation.
        .AddFluentValidationConfig()
        // Register global exception handling for consistent API error responses.
        .AddExceptionHandlerConfiguration()
        // Register and configure the application's database connection and DbContext.
        .AddDatabaseConfiguration(configuration);

        // Register the hosted background service that processes pending outbox messages.
        services.AddHostedService<OutboxBackgroundService>();

        // Register SignalR services for real-time communication.
        services.AddSignalR();

        // Define a CORS policy for the local notification test client.
        // Allow requests from its development origin, including any headers and HTTP methods.
        // redentials are enabled so browser-based SignalR clients can use authenticated connections.
        services.AddCors(options =>
        {
            options.AddPolicy("NotifyXPolicy", policy =>
            {
                policy
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        // Register Redis as the distributed cache and use a NotifyX-specific key prefix.
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration =
                configuration.GetConnectionString("Redis");

            options.InstanceName = "NotifyX:";
        });

        return services;
    }

    /*
        -- Add database dependencies to the container.
        -- This method configures the database context and adds it to the service collection.
        -- It retrieves the connection string from the configuration and uses it to configure the DbContext.
    */
    private static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                /*
                 * Configure SQL Server's transient-failure retry strategy.
                 * Retry failed database operations up to five times, waiting up to
                 * ten seconds between attempts. No additional SQL error numbers
                 * are specified; the provider uses its default transient-error list.
                 */
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null
                    );
                }
            )
        );
        return services;
    }

    /*
        -- Add default configuration to the container.
        -- This method registers the global exception handler and problem details services.
    */
    private static IServiceCollection AddExceptionHandlerConfiguration(this IServiceCollection services)
    {
        // Add Configuration for Add Global Exception Handler
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }


    /*
        -- Add FluentValidation dependencies to the container.
        -- This method configures automatic model validation and registers all validators
        -- from the executing assembly so request DTOs are validated consistently.
    */
    private static IServiceCollection AddFluentValidationConfig(
        this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(
                Assembly.GetExecutingAssembly());

        return services;
    }

    /*
        -- Add scoped dependencies to the container.
        -- This method registers repositories and services with a scoped lifetime.
    */
    private static IServiceCollection AddScopedConfiguration(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<
            INotificationRepository,
            NotificationRepository>();

        services.AddScoped<
            IIdempotencyKeysRepository,
            IdempotencyKeysRepository>();

        services.AddScoped<
            INotificationRecipientRepository,
            NotificationRecipientRepository>();

        // Services
        services.AddScoped<
            INotificationService,
            NotificationService>();

        services.AddScoped<
            IIdempotencyService,
            IdempotencyService>();

        services.AddScoped<
            IOutboxService,
            OutboxService>();

        services.AddScoped<
            INotificationRecipientService,
            NotificationRecipientService>();

        services.AddScoped<NotificationFactory>();
        services.AddScoped<INotificationCacheService, NotificationCacheService>();

        // Publisher
        services.AddScoped<
            INotificationPublisher,
            SignalRNotificationPublisher>();
        return services;
    }
}