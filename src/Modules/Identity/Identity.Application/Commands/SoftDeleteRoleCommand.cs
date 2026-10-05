namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;

public sealed record SoftDeleteRoleCommand(Guid RoleId) : IRequest<Result>;