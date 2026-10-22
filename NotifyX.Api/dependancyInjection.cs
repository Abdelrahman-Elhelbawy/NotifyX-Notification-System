using Microsoft.EntityFrameworkCore;
using NotifyX.Api.Common.Exceptions;
using NotifyX.Api.Data;

using NotifyX.Api.Repositories.Notifications;
using NotifyX.Api.Repository.Notifcations;
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
            .AddDefaultConfiguration();

        // Register SignalR services for real-time communication.
        services.AddSignalR();

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
            options.UseSqlServer(connectionString));

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
        -- Add scoped dependencies to the container.
        -- This method registers repositories and services with a scoped lifetime.
    */
    private static IServiceCollection AddScopedConfiguration(this IServiceCollection services)
    {
        // Register Repositories
        services.AddScoped<
            INotificationRepository,
            NotificationRepository>();

        // Register Services
        services.AddScoped<
            INotificationService,
            NotificationService>();

        return services;
    }
}