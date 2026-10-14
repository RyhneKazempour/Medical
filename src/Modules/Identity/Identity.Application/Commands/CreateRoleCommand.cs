namespace Identity.Application.Commands;

using Identity.Application.Authentication;
using MediatR;
using Shared.Domain.Common;

public sealed record CreateRoleCommand(
    string Name,
    string? Description) : IRequest<Result<RoleResponse>>;
