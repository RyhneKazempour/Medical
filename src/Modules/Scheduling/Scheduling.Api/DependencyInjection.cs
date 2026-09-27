namespace MyApp.Scheduling.Api;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddSchedulingApi(this IServiceCollection services)
    {
        return services;
    }

    public static IEndpointRouteBuilder MapSchedulingApi(this IEndpointRouteBuilder app)
    {
        return app;
    }
}
