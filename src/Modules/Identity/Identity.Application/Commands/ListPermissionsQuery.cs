namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;
using MyApp.Identity.Application.Authentication;

public sealed record ListPermissionsQuery : IRequest<Result<IReadOnlyList<PermissionResponse>>>;