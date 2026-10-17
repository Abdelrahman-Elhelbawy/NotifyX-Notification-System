using Microsoft.EntityFrameworkCore;
using NotifyX.Api.Data;

namespace NotifyX.Api;

public static class DependencyInjection
{
    /*
        -- Add dependencies to the container.
        -- This method is an extension method for IServiceCollection that adds various dependencies to the service collection.
        -- It calls the AddDatabaseDependencies method to add database-related dependencies.
    */
    public static IServiceCollection AddDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabaseDependencies(configuration);
        return services;
    }

    /*
        -- Add database dependencies to the container.
        -- This method configures the database context and adds it to the service collection.
        -- It retrieves the connection string from the configuration and uses it to configure the DbContext.
    */
    private static IServiceCollection AddDatabaseDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}