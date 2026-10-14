namespace Identity.Application.Commands;

using MediatR;

using Shared.Domain.Common;
public sealed record RemovePermissionFromRoleCommand(
    Guid RoleId,
    Guid PermissionId) : IRequest<Result>;
