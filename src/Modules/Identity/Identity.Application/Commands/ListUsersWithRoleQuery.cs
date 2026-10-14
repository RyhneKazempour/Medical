namespace Identity.Application.Commands;

using Identity.Application.Authentication;
using MediatR;
using Shared.Domain.Common;

public sealed record ListUsersWithRoleQuery(Guid RoleId) : IRequest<Result<IReadOnlyList<UserResponse>>>;
