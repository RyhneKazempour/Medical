namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;

public sealed record RestoreUserCommand(Guid UserId) : IRequest<Result>;