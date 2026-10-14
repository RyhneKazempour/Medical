namespace Identity.Application.Commands;

using Identity.Application.Authentication;
using MediatR;
using Shared.Domain.Common;

public sealed record CreatePermissionCommand(
    string Resource,
    string Action,
    string? Description) : IRequest<Result<PermissionResponse>>;
