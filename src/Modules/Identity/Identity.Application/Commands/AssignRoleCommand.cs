namespace Identity.Application.Commands;

using Identity.Application.Authentication;
using Identity.Domain.ValueObjects;
using MediatR;
using Shared.Domain.Common;

public sealed record AssignRoleCommand(
    Guid UserId,
    Guid RoleId,
    ScopeType ScopeType,
    Guid ScopeId) : IRequest<Result<UserRoleDto>>;
