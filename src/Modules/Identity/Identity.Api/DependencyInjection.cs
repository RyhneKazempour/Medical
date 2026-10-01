namespace MyApp.Identity.Api;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Application.Authentication;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApi(this IServiceCollection services)
    {
        return services;
    }

    public static IEndpointRouteBuilder MapIdentityApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/identity").WithTags("Identity");

        group.MapPost("/login", async (LoginCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
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
        })
        .WithName("Login")
        .WithOpenApi()
        .AllowAnonymous();

        group.MapPost("/refresh", async (RefreshTokenCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
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
        })
        .WithName("RefreshToken")
        .WithOpenApi()
        .AllowAnonymous();

        group.MapGet("/me", (Microsoft.AspNetCore.Http.HttpContext httpContext) =>
        {
            var user = httpContext.User;
            return Results.Ok(new
            {
                user_id = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                email = user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
                roles = user.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToArray()
            });
        })
        .WithName("GetCurrentUser")
        .WithOpenApi()
        .RequireAuthorization();

        return app;
    }
}
