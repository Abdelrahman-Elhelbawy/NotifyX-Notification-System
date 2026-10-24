using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using NotifyX.Api.Common.Exceptions;
using NotifyX.Api.Data;
using NotifyX.Api.Factories;
using NotifyX.Api.Publishers;
using NotifyX.Api.Repositories.Notifications;
using NotifyX.Api.Repository.IdempotencyKeys;
using NotifyX.Api.Repository.IdempotencyKeysRepository;
using NotifyX.Api.Repository.Notifcations;
using NotifyX.Api.Services.Idempotency;
using NotifyX.Api.Services.Notifications;

namespace NotifyX.Api;

public static class DependencyInjection
{
    /*
        -- Add dependencies to the container.
        -- This method is an extension method for IServiceCollection that adds various dependencies to the service collection.
        -- It calls the AddDatabaseDependencies method to add database-related dependencies.
    */
    public static IServiceCollection AddConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddScopedConfiguration()
            .AddDatabaseConfiguration(configuration)
            .AddDefaultConfiguration()
            .AddFluentValidationConfig();

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
    private static IServiceCollection AddDefaultConfiguration(this IServiceCollection services)
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

        // Services
        services.AddScoped<
            INotificationService,
            NotificationService>();

        services.AddScoped<
            IIdempotencyService, 
            IdempotencyService>();
            
        services.AddScoped<NotificationFactory>();

        // Publisher
        services.AddScoped<
            INotificationPublisher,
            SignalRNotificationPublisher>();
        return services;
    }
}