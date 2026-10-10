namespace MyApp.Identity.Api.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.OpenApi;
using System.Security.Claims;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/identity")
            .WithTags("Users");

        group.MapGet("/me", GetCurrent)
            .WithName("GetCurrentUser")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(object))
            .RequireAuthorization();

        return app;
    }

    private static IResult GetCurrent(ClaimsPrincipal user)
    {
        return Results.Ok(new
        {
            user_id = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            email = user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
            roles = user.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToArray()
        });
    }
}