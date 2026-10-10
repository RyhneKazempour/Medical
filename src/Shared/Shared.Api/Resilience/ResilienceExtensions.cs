using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Shared.Api.Resilience;

namespace MyApp.Shared.Api.Resilience;

/// <summary>
/// Extension methods for registering resilience services with DI.
/// </summary>
public static class ResilienceExtensions
{
    /// <summary>
    /// Adds resilience services to the DI container and binds configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddResilienceConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ResilienceOptions>(configuration.GetSection("Resilience"));
        
        return services;
    }
}