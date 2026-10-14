namespace Identity.Application.Commands;

using Identity.Domain.ValueObjects;
using MediatR;
using Shared.Domain.Common;

public sealed record RemoveRoleCommand(
    Guid UserId,
    Guid RoleId,
    ScopeType ScopeType,
    Guid ScopeId) : IRequest<Result>;
