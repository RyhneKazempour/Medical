using Microsoft.AspNetCore.Http;


namespace MyApp.Shared.Api.RateLimiting;

public static class ClientPartitionResolver
{
    public static string GetIp(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";
    }
}