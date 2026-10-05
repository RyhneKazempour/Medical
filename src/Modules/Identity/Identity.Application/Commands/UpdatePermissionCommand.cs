namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;
using MyApp.Identity.Application.Authentication;

public sealed record UpdatePermissionCommand(
    Guid PermissionId,
    string? Resource,
    string? Action,
    string? Description) : IRequest<Result<PermissionResponse>>;