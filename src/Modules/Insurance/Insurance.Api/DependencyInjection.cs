namespace MyApp.Insurance.Api;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInsuranceApi(this IServiceCollection services)
    {
        return services;
    }

    public static IEndpointRouteBuilder MapInsuranceApi(this IEndpointRouteBuilder app)
    {
        return app;
    }
}
