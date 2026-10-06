namespace MyApp.Shared.Infrastructure.Observability.Metrics;

using System.Diagnostics.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class MeterRegistry
{
    public const string MeterName = "MyApp.BusinessMetrics";
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessMetrics(this IServiceCollection services)
    {
        services.AddSingleton(_ => new AuthenticationMetrics());

        return services;
    }
}

public sealed class AuthenticationMetrics
{
    private readonly Meter _meter;
    private readonly Counter<long> _loginAttempts;

    public AuthenticationMetrics()
    {
        _meter = new Meter(MeterRegistry.MeterName, "1.0.0");
        _loginAttempts = _meter.CreateCounter<long>(
            name: "myapp_identity_login_attempts_total",
            description: "Number of login attempts by outcome",
            unit: "1");
    }

    public void RecordLoginAttempt(bool success)
    {
        _loginAttempts.Add(1, new KeyValuePair<string, object?>("outcome", success ? "success" : "failure"));
    }
}