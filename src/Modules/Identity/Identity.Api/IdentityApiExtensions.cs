namespace Identity.Api;

using Identity.Api.Endpoints;
using Microsoft.AspNetCore.Routing;

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
