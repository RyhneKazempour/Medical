namespace MyApp.Shared.Api.Bulkhead;

/// <summary>
/// Configuration options for a bulkhead.
/// </summary>
public class BulkheadOptions
{
    /// <summary>
    /// Maximum number of concurrent executions allowed.
    /// </summary>
    public int PermitLimit { get; set; } = 10;

    /// <summary>
    /// Maximum number of executions that can be queued.
    /// </summary>
    public int QueueLimit { get; set; } = 20;
}