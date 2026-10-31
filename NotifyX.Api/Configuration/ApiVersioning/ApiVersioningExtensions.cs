using Asp.Versioning;

namespace NotifyX.Api.Configuration.ApiVersioning;

public static class ApiVersioningExtensions
{
    /// <summary>
    /// Configures API versioning using URL path segments.
    /// </summary>
    public static IServiceCollection AddApiVersioningConfiguration(
        this IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                // Set the default API version.
                options.DefaultApiVersion = new ApiVersion(1, 0);

                // Assume the default version when no version is specified.
                options.AssumeDefaultVersionWhenUnspecified = true;

                // Report supported API versions in response headers.
                options.ReportApiVersions = true;

                // Read the API version from the URL.
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                // Format API versions as v1, v2, etc.
                options.GroupNameFormat = "'v'VVV";

                // Replace the version placeholder in route templates.
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}