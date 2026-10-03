using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;


namespace MyApp.Shared.Api.RateLimiting;


public static class RateLimitingConfiguration
{
    public static IServiceCollection AddRateLimitingConfiguration(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Default policy for all endpoints
            options.AddPolicy(
                RateLimitPolicies.Default,
                context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        ClientPartitionResolver.GetIp(context),
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }));


            // Login
            options.AddPolicy(
                RateLimitPolicies.IdentityAuth,
                context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        ClientPartitionResolver.GetIp(context),
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));

            // Refresh token
            options.AddPolicy(
                RateLimitPolicies.IdentityRefresh,
                context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        ClientPartitionResolver.GetIp(context),
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 30,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));
        });

        return services;
    }
}