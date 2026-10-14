namespace Identity.Application.Commands;

using MediatR;
using Shared.Domain.Common;

public sealed record SoftDeleteUserCommand(Guid UserId) : IRequest<Result>;
