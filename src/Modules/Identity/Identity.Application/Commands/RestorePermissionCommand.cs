namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;

public sealed record RestorePermissionCommand(Guid PermissionId) : IRequest<Result>;