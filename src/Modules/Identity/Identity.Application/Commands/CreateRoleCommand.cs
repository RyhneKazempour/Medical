namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;
using MyApp.Identity.Application.Authentication;

public sealed record CreateRoleCommand(
    string Name,
    string? Description) : IRequest<Result<RoleResponse>>;