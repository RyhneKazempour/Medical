namespace MyApp.Identity.Application.Handlers;

using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Domain;
using MyApp.Shared.Application.Abstractions;

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