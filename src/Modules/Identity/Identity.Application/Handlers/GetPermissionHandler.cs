namespace Identity.Application.Handlers;

using Identity.Application.Abstractions;
using Identity.Application.Authentication;
using Identity.Application.Commands;
using MediatR;
using Shared.Domain.Common;

internal sealed class GetPermissionHandler : IRequestHandler<GetPermissionQuery, Result<PermissionResponse>>
{
    private readonly IPermissionRepository _permissionRepository;

    public GetPermissionHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<Result<PermissionResponse>> Handle(GetPermissionQuery request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
        if (permission is null)
            return Result<PermissionResponse>.Failure(new Error("Permission.NotFound", "Permission not found."));

        return Result<PermissionResponse>.Success(new PermissionResponse(
            permission.Id,
            permission.Resource,
            permission.Action,
            permission.Description,
            permission.IsDeleted));
    }
}
