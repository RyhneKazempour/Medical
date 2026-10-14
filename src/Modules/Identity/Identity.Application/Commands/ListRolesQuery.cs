namespace Identity.Application.Commands;

using Identity.Application.Authentication;
using MediatR;
using Shared.Domain.Common;

public sealed record ListRolesQuery : IRequest<Result<IReadOnlyList<RoleResponse>>>;
