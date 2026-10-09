using MyApp.Shared.Api.Bulkhead;

namespace MyApp.Shared.Api.Resilience;

/// <summary>
/// Root configuration section for resilience policies.
/// </summary>
public class ResilienceOptions
{
    public Dictionary<string, DependencyResilienceOptions> Dependencies { get; set; } = new();
}

/// <summary>
/// Configuration for a single external dependency.
/// </summary>
public class DependencyResilienceOptions
{
    /// <summary>
    /// Maximum time in milliseconds an operation may run.
    /// </summary>
    public int? TimeoutMilliseconds { get; set; }

    /// <summary>
    /// Retry configuration.
    /// </summary>
    public RetryOptions? Retry { get; set; }

    /// <summary>
    /// Circuit breaker configuration.
    /// </summary>
    public CircuitBreakerOptions? CircuitBreaker { get; set; }

    /// <summary>
    /// Bulkhead configuration.
    /// </summary>
    public BulkheadOptions? Bulkhead { get; set; }
}

/// <summary>
/// Retry configuration.
/// </summary>
public class RetryOptions
{
    /// <summary>
    /// Maximum number of retry attempts (excluding the original attempt).
    /// </summary>
    public int MaxAttempts { get; set; } = 2;

    /// <summary>
    /// Base delay in milliseconds for exponential backoff.
    /// </summary>
    public int DelayMilliseconds { get; set; } = 200;

    /// <summary>
    /// Maximum backoff delay in milliseconds.
    /// </summary>
    public int MaxDelayMilliseconds { get; set; } = 2000;

    /// <summary>
    /// Use jitter to avoid thundering herd.
    /// </summary>
    public bool UseJitter { get; set; } = true;
}

/// <summary>
/// Circuit breaker configuration.
/// </summary>
public class CircuitBreakerOptions
{
    /// <summary>
    /// Fraction of failures that will trigger the circuit breaker.
    /// </summary>
    public double FailureRatio { get; set; } = 0.5;

    /// <summary>
    /// Minimum number of requests before the circuit breaker can trip.
    /// </summary>
    public int MinimumThroughput { get; set; } = 10;

    /// <summary>
    /// Duration in milliseconds over which failures are sampled.
    /// </summary>
    public int SamplingDurationMilliseconds { get; set; } = 30000;

    /// <summary>
    /// Duration in milliseconds the circuit stays open before transitioning to half-open.
    /// </summary>
    public int BreakDurationMilliseconds { get; set; } = 15000;
}
