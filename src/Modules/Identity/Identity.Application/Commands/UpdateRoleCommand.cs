namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;
using MyApp.Identity.Application.Authentication;

public sealed record UpdateRoleCommand(
    Guid RoleId,
    string? Name,
    string? Description,
    bool? IsActive) : IRequest<Result<RoleResponse>>;