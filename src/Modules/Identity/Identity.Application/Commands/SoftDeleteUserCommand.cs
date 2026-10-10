namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;

public sealed record SoftDeleteUserCommand(Guid UserId) : IRequest<Result>;