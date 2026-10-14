namespace Identity.Application.Handlers;

using Identity.Application.Abstractions;
using Identity.Application.Authentication;
using Identity.Application.Commands;
using MediatR;
using Shared.Domain.Common;

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
