namespace MyApp.Identity.Api.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.OpenApi;
using MediatR;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Api.RateLimiting;

public static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/identity")
            .WithTags("Authentication");

        group.MapPost("/login", Login)
            .WithName("Login")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(object))
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireRateLimiting(RateLimitPolicies.IdentityRefresh)
            .AllowAnonymous();

        group.MapPost("/refresh", RefreshToken)
            .WithName("RefreshToken")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(object))
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireRateLimiting(RateLimitPolicies.IdentityRefresh)
            .AllowAnonymous();

        return app;
    }

    private static string GetClientIp(HttpContext httpContext)
    {
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            return forwardedFor.Split(',').First().Trim();
        }

        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static async Task<IResult> Login(
        LoginCommand command,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command with { ClientIpAddress = GetClientIp(httpContext) }, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(new
            {
                access_token = result.Value.AccessToken.Token,
                token_type = "Bearer",
                expires_in = result.Value.AccessToken.ExpiresIn,
                refresh_token = result.Value.RefreshToken,
                refresh_expires_in = result.Value.RefreshTokenExpiresIn
            })
            : Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description });
    }

    private static async Task<IResult> RefreshToken(
        RefreshTokenCommand command,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command with { ClientIpAddress = GetClientIp(httpContext) }, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(new
            {
                access_token = result.Value.AccessToken.Token,
                token_type = "Bearer",
                expires_in = result.Value.AccessToken.ExpiresIn,
                refresh_token = result.Value.RefreshToken,
                refresh_expires_in = result.Value.RefreshTokenExpiresIn
            })
            : Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description });
    }
}
