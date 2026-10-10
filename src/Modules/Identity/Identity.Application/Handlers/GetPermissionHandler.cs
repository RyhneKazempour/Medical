namespace MyApp.Identity.Application.Handlers;

using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Domain;

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