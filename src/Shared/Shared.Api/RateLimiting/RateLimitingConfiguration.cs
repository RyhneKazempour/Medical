using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Shared.Api.Bulkhead;
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

            // --- Rate Limit Policies ---

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

            // OTP uses the fixed window limiter (legacy config).
            // OTP must never be bulkheaded; it is blocked 429 at the rate limiter instead.
            options.AddPolicy(
                RateLimitPolicies.IdentityOtp,
                context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        ClientPartitionResolver.GetIp(context),
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));

            // --- Bulkhead Policies ---
            // Use concurrency limiters to isolate resource-intensive operations.
            // These are separate from the rate limit policies above.

            options.AddConcurrencyLimiter(BulkheadPolicies.Payment,
                _ => new ConcurrencyLimiterOptions
                {
                    PermitLimit = 10,
                    QueueLimit = 20,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });

            options.AddConcurrencyLimiter(BulkheadPolicies.Notification,
                _ => new ConcurrencyLimiterOptions
                {
                    PermitLimit = 10,
                    QueueLimit = 20,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
        });

        return services;
    }
}