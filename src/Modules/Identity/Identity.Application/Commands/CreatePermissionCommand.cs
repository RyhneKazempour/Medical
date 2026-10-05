namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;
using MyApp.Identity.Application.Authentication;

public sealed record CreatePermissionCommand(
    string Resource,
    string Action,
    string? Description) : IRequest<Result<PermissionResponse>>;
