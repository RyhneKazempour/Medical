namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;

public sealed record RestoreRoleCommand(Guid RoleId) : IRequest<Result>;