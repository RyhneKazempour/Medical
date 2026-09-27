namespace MyApp.Identity.Api;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApi(this IServiceCollection services)
    {
        return services;
    }

    public static IEndpointRouteBuilder MapIdentityApi(this IEndpointRouteBuilder app)
    {
        return app;
    }
}
