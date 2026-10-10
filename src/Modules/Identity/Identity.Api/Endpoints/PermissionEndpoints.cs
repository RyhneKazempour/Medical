namespace MyApp.Identity.Api.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.OpenApi;
using MediatR;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Api.RateLimiting;

public static class PermissionEndpoints
{
    public static IEndpointRouteBuilder MapPermissionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/identity/permissions")
            .WithTags("Permissions")

            ;

        group.MapPost("/", CreatePermission)
            .WithName("CreatePermission")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(PermissionResponse))
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{permissionId:guid}", GetPermission)
            .WithName("GetPermission")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(PermissionResponse))
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{permissionId:guid}", UpdatePermission)
            .WithName("UpdatePermission")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(PermissionResponse))
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{permissionId:guid}", SoftDeletePermission)
            .WithName("SoftDeletePermission")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/", ListPermissions)
            .WithName("ListPermissions")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK, typeof(IReadOnlyList<PermissionResponse>))
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/{roleId:guid}/assign/{permissionId:guid}", AssignPermissionToRole)
            .WithName("AssignPermissionToRole")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{roleId:guid}/remove/{permissionId:guid}", RemovePermissionFromRole)
            .WithName("RemovePermissionFromRole")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/{permissionId:guid}/restore", RestorePermission)
            .WithName("RestorePermission")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> CreatePermission(
        CreatePermissionCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description });
    }

    private static async Task<IResult> GetPermission(
        Guid permissionId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetPermissionQuery(permissionId);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.Code switch
            {
                "Permission.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }

    private static async Task<IResult> UpdatePermission(
        Guid permissionId,
        UpdatePermissionCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var updatedCommand = command with { PermissionId = permissionId };
        var result = await sender.Send(updatedCommand, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.Code switch
            {
                "Permission.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }

    private static async Task<IResult> SoftDeletePermission(
        Guid permissionId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new SoftDeletePermissionCommand(permissionId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok()
            : result.Error.Code switch
            {
                "Permission.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }

    private static async Task<IResult> ListPermissions(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new ListPermissionsQuery();
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description });
    }

    private static async Task<IResult> AssignPermissionToRole(
        Guid roleId,
        Guid permissionId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new AssignPermissionToRoleCommand(roleId, permissionId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok()
            : result.Error.Code switch
            {
                "Role.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                "Permission.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }

    private static async Task<IResult> RemovePermissionFromRole(
        Guid roleId,
        Guid permissionId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RemovePermissionFromRoleCommand(roleId, permissionId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok()
            : result.Error.Code switch
            {
                "Role.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                "Permission.NotFound" => Results.NotFound(new { error = result.Error.Code, message = result.Error.Description }),
                _ => Results.BadRequest(new { error = result.Error.Code, message = result.Error.Description })
            };
    }
}