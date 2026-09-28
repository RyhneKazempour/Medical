namespace MyApp.Identity.Domain.Entities;

using MyApp.Shared.Domain;

public sealed class RolePermission : AuditableEntity
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }

    public Role? Role { get; private set; }
    public Permission? Permission { get; private set; }

    private RolePermission() { }

    private RolePermission(Guid id, Guid roleId, Guid permissionId)
        : base(id)
    {
        RoleId = roleId;
        PermissionId = permissionId;
        IsDeleted = false;
    }

    public static Result<RolePermission> Create(Guid roleId, Guid permissionId)
    {
        if (roleId == Guid.Empty)
            return Result<RolePermission>.Failure(new Error("RolePermission.RoleIdRequired", "Role ID is required."));

        if (permissionId == Guid.Empty)
            return Result<RolePermission>.Failure(new Error("RolePermission.PermissionIdRequired", "Permission ID is required."));

        var rolePermission = new RolePermission(Guid.NewGuid(), roleId, permissionId);
        return Result<RolePermission>.Success(rolePermission);
    }

    public void Restore()
    {
        IsDeleted = false;
    }

    public void SoftDelete() => IsDeleted = true;
}
