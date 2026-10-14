namespace Identity.Application.Commands;

using MediatR;
using Shared.Domain.Common;

public sealed record RestoreRoleCommand(Guid RoleId) : IRequest<Result>;
