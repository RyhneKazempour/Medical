namespace Identity.Application.Handlers;

using Identity.Application.Abstractions;
using Identity.Application.Commands;
using Identity.Domain.Entities;
using MediatR;
using Shared.Application.Abstractions;
using Shared.Domain.Common;

internal sealed class AssignPermissionToRoleHandler : IRequestHandler<AssignPermissionToRoleCommand, Result>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignPermissionToRoleHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
            return Result.Failure(new Error("Role.NotFound", "Role not found."));

        if (!role.IsActive)
            return Result.Failure(new Error("Role.Inactive", "Role is inactive."));

        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
        if (permission is null)
            return Result.Failure(new Error("Permission.NotFound", "Permission not found."));

        if (permission.IsDeleted)
            return Result.Failure(new Error("Permission.Deleted", "Permission has been deleted."));

        var existingAssignment = await _roleRepository.GetPermissionOfRole(
            request.RoleId, request.PermissionId, cancellationToken);

        if (existingAssignment is not null && !existingAssignment.IsDeleted)
            return Result.Failure(new Error("RolePermission.DuplicateAssignment", "Permission is already assigned to this role."));

        RolePermission rolePermission;
        if (existingAssignment is not null && existingAssignment.IsDeleted)
        {
            existingAssignment.Restore();
            rolePermission = existingAssignment;
        }
        else
        {
            var createResult = RolePermission.Create(request.RoleId, request.PermissionId);
            if (createResult.IsFailure)
                return Result.Failure(createResult.Error);

            rolePermission = createResult.Value;
        }

        await _roleRepository.AddPermissionToRole(rolePermission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
