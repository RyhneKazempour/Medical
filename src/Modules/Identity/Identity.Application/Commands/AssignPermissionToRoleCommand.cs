namespace Identity.Application.Commands;

using MediatR;
using Shared.Domain.Common;

public sealed record AssignPermissionToRoleCommand(
    Guid RoleId,
    Guid PermissionId) : IRequest<Result>;
