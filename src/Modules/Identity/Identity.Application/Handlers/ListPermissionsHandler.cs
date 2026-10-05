namespace MyApp.Identity.Application.Handlers;

using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Domain;

internal sealed class ListPermissionsHandler : IRequestHandler<ListPermissionsQuery, Result<IReadOnlyList<PermissionResponse>>>
{
    private readonly IPermissionRepository _permissionRepository;

    public ListPermissionsHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<Result<IReadOnlyList<PermissionResponse>>> Handle(ListPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);
        var response = permissions.Select(p => new PermissionResponse(
            p.Id,
            p.Resource,
            p.Action,
            p.Description,
            p.IsDeleted)).ToList();

        return Result<IReadOnlyList<PermissionResponse>>.Success(response);
    }
}