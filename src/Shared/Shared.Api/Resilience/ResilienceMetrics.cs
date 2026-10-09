using System.Diagnostics.Metrics;

namespace MyApp.Shared.Api.Resilience;

/// <summary>
/// Meter source for resilience observability metrics.
/// </summary>
public static class ResilienceMetrics
{
    private static readonly Meter _meter = new(
        "MyApp.Resilience",
        "1.0.0");

    /// <summary>
    /// Counter for retry attempts across all dependencies.
    /// </summary>
    public static readonly Counter<long> RetryAttempts =
        _meter.CreateCounter<long>(
            name: "myapp_resilience_retry_attempts_total",
            description: "Total number of retry attempts across all dependencies",
            unit: "1");

    /// <summary>
    /// Counter for timeout occurrences across all dependencies.
    /// </summary>
    public static readonly Counter<long> Timeouts =
        _meter.CreateCounter<long>(
            name: "myapp_resilience_timeouts_total",
            description: "Total number of timeout occurrences across all dependencies",
            unit: "1");

    /// <summary>
    /// Counter for circuit breaker transitions to OPEN state.
    /// </summary>
    public static readonly Counter<long> CircuitBreakerOpened =
        _meter.CreateCounter<long>(
            name: "myapp_resilience_circuit_breaker_opened_total",
            description: "Total number of circuit breaker transitions to OPEN state",
            unit: "1");

    /// <summary>
    /// Counter for circuit breaker transitions to CLOSED state.
    /// </summary>
    public static readonly Counter<long> CircuitBreakerClosed =
        _meter.CreateCounter<long>(
            name: "myapp_resilience_circuit_breaker_closed_total",
            description: "Total number of circuit breaker transitions to CLOSED state",
            unit: "1");

/// <summary>
    /// Counter for circuit breaker transitions to HALF-OPEN state.
    /// </summary>
    public static readonly Counter<long> CircuitBreakerHalfOpen =
        _meter.CreateCounter<long>(
            name: "myapp_resilience_circuit_breaker_half_open_total",
            description: "Total number of circuit breaker transitions to HALF-OPEN state",
            unit: "1");

    /// <summary>
    /// Counter for bulkhead rejected requests.
    /// </summary>
    public static readonly Counter<long> BulkheadRejected =
        _meter.CreateCounter<long>(
            name: "myapp_resilience_bulkhead_rejected_total",
            description: "Total number of requests rejected by bulkhead",
            unit: "1");

    /// <summary>
    /// Counter for bulkhead active requests (running within permit limit).
    /// </summary>
    public static readonly Counter<long> BulkheadActive =
        _meter.CreateCounter<long>(
            name: "myapp_resilience_bulkhead_active_total",
            description: "Total number of requests currently active within bulkhead permit",
            unit: "1");

    /// <summary>
    /// Counter for bulkhead queued requests.
    /// </summary>
    public static readonly Counter<long> BulkheadQueued =
        _meter.CreateCounter<long>(
            name: "myapp_resilience_bulkhead_queued_total",
            description: "Total number of requests queued waiting for bulkhead permit",
            unit: "1");
}