namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;

public sealed record AssignPermissionToRoleCommand(
    Guid RoleId,
    Guid PermissionId) : IRequest<Result>;