namespace Identity.Application.Handlers;

using Identity.Application.Abstractions;
using Identity.Application.Commands;
using MediatR;
using Shared.Application.Abstractions;
using Shared.Domain.Common;

internal sealed class RemovePermissionFromRoleHandler : IRequestHandler<RemovePermissionFromRoleCommand, Result>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemovePermissionFromRoleHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemovePermissionFromRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
            return Result.Failure(new Error("Role.NotFound", "Role not found."));

        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
        if (permission is null)
            return Result.Failure(new Error("Permission.NotFound", "Permission not found."));

        var assignment = await _roleRepository.GetPermissionOfRole(
            request.RoleId, request.PermissionId, cancellationToken);

        if (assignment is null || assignment.IsDeleted)
            return Result.Failure(new Error("RolePermission.NotFound", "Permission is not assigned to this role."));

        assignment.SoftDelete();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
