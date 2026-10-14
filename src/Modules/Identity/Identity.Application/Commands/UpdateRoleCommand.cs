namespace Identity.Application.Commands;

using Identity.Application.Authentication;
using MediatR;
using Shared.Domain.Common;

public sealed record UpdateRoleCommand(
    Guid RoleId,
    string? Name,
    string? Description,
    bool? IsActive) : IRequest<Result<RoleResponse>>;
