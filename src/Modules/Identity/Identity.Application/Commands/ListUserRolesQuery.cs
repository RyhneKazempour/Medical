namespace Identity.Application.Commands;

using Identity.Application.Authentication;
using MediatR;
using Shared.Domain.Common;

public sealed record ListUserRolesQuery(Guid UserId) : IRequest<Result<IReadOnlyList<UserRoleDto>>>;
