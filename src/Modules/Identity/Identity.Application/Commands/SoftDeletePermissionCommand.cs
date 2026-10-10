namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;

public sealed record SoftDeletePermissionCommand(Guid PermissionId) : IRequest<Result>;