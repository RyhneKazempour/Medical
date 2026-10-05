namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;
using MyApp.Identity.Application.Authentication;

public sealed record ListUsersWithRoleQuery(Guid RoleId) : IRequest<Result<IReadOnlyList<UserResponse>>>;