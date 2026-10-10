namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;

public sealed record RemoveRoleCommand(
    Guid UserId,
    Guid RoleId,
    MyApp.Identity.Domain.ValueObjects.ScopeType ScopeType,
    Guid ScopeId) : IRequest<Result>;