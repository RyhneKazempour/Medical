namespace MyApp.Identity.Api;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MyApp.Identity.Api.Endpoints;

public static class IdentityApiExtensions
{
    public static IEndpointRouteBuilder MapIdentityApi(this IEndpointRouteBuilder app)
    {
        app.MapAuthenticationEndpoints();
        app.MapUserEndpoints();
        app.MapPermissionEndpoints();

        return app;
    }
}