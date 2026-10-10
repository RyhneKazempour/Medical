namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;
using MyApp.Identity.Application.Authentication;

public sealed record AssignRoleCommand(
    Guid UserId,
    Guid RoleId,
    MyApp.Identity.Domain.ValueObjects.ScopeType ScopeType,
    Guid ScopeId) : IRequest<Result<UserRoleDto>>;