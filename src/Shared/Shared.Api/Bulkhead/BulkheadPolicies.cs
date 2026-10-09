namespace MyApp.Shared.Api.Bulkhead;

/// <summary>
/// Named bulkhead policies for resource isolation.
/// Maps to the "Bulkhead" configuration section by key.
/// </summary>
public static class BulkheadPolicies
{
    public const string Payment = "Payment";

    public const string Notification = "Notification";
}
