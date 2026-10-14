namespace Identity.Application.Commands;

using MediatR;
using Shared.Domain.Common;

public sealed record RestoreUserCommand(Guid UserId) : IRequest<Result>;
