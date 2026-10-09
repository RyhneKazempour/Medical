using MyApp.Shared.Api.Bulkhead;
using MyApp.Shared.Api.Resilience;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace MyApp.Shared.Api.Resilience;

/// <summary>
/// Static factory for creating resilience pipelines.
/// </summary>
public static class ResiliencePolicies
{
    /// <summary>
    /// Creates a timeout policy for a dependency.
    /// </summary>
    /// <param name="builder">The resilience pipeline builder.</param>
    /// <param name="options">Timeout configuration.</param>
    /// <returns>A Polly resilience pipeline segment for timeout.</returns>
    public static ResiliencePipelineBuilder AddTimeoutPolicy(
        this ResiliencePipelineBuilder builder,
        DependencyResilienceOptions options)
    {
        if (options?.TimeoutMilliseconds.HasValue == true)
        {
            return builder.AddTimeout(TimeSpan.FromMilliseconds(options.TimeoutMilliseconds.Value));
        }

        return builder;
    }

    /// <summary>
    /// Creates a retry policy for transient failures.
    /// </summary>
    /// <param name="builder">The resilience pipeline builder.</param>
    /// <param name="options">Retry configuration.</param>
    /// <returns>A Polly resilience pipeline segment for retry.</returns>
    public static ResiliencePipelineBuilder AddRetryPolicy(
        this ResiliencePipelineBuilder builder,
        RetryOptions options)
    {
        if (options == null || options.MaxAttempts <= 0)
        {
            return builder;
        }

        var retryOptions = new RetryStrategyOptions
        {
            MaxRetryAttempts = options.MaxAttempts,
            Delay = TimeSpan.FromMilliseconds(options.DelayMilliseconds),
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = options.UseJitter,
            OnRetry = static args => ValueTask.CompletedTask
        };

        return builder.AddRetry(retryOptions);
    }

    /// <summary>
    /// Creates a circuit breaker policy.
    /// </summary>
    /// <param name="builder">The resilience pipeline builder.</param>
    /// <param name="options">Circuit breaker configuration.</param>
    /// <returns>A Polly resilience pipeline segment for circuit breaker.</returns>
    public static ResiliencePipelineBuilder AddCircuitBreakerPolicy(
        this ResiliencePipelineBuilder builder,
        CircuitBreakerOptions options)
    {
        if (options == null)
        {
            return builder;
        }

        var circuitBreakerOptions = new CircuitBreakerStrategyOptions
        {
            FailureRatio = options.FailureRatio,
            MinimumThroughput = options.MinimumThroughput,
            SamplingDuration = TimeSpan.FromMilliseconds(options.SamplingDurationMilliseconds),
            BreakDuration = TimeSpan.FromMilliseconds(options.BreakDurationMilliseconds)
        };

        return builder.AddCircuitBreaker(circuitBreakerOptions);
    }

    /// <summary>
    /// Creates a bulkhead policy using Polly's ConcurrencyLimiter.
    /// </summary>
    /// <param name="builder">The resilience pipeline builder.</param>
    /// <param name="options">Bulkhead configuration options.</param>
    /// <returns>A Polly resilience pipeline segment for bulkhead.</returns>
    public static ResiliencePipelineBuilder AddBulkheadPolicy(
        this ResiliencePipelineBuilder builder,
        BulkheadOptions options)
    {
        if (options == null)
        {
            return builder;
        }

        return builder.AddConcurrencyLimiter(options.PermitLimit, options.QueueLimit);
    }
}