namespace Identity.Application.Commands;

using MediatR;
using Shared.Domain.Common;

public sealed record RestorePermissionCommand(Guid PermissionId) : IRequest<Result>;
