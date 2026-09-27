namespace MyApp.Billing.Api;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddBillingApi(this IServiceCollection services)
    {
        return services;
    }

    public static IEndpointRouteBuilder MapBillingApi(this IEndpointRouteBuilder app)
    {
        return app;
    }
}
