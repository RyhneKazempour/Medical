namespace MyApp.Medical.Api;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddMedicalApi(this IServiceCollection services)
    {
        return services;
    }

    public static IEndpointRouteBuilder MapMedicalApi(this IEndpointRouteBuilder app)
    {
        return app;
    }
}
