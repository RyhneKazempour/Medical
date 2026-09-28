namespace MyApp.Identity.Domain.Entities;

using MyApp.Shared.Domain;

public sealed class Role : AuditableActivatableEntity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    private UserRole[] _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

    private RolePermission[] _rolePermissions = [];
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

    private Role() { }

    private Role(Guid id, string name, string? description)
        : base(id)
    {
        Name = name;
        Description = description;
        IsActive = true;
        IsDeleted = false;
    }

    public static Result<Role> Create(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Role>.Failure(new Error("Role.NameRequired", "Role name is required."));

        var role = new Role(Guid.NewGuid(), name.Trim(), description?.Trim());
        return Result<Role>.Success(role);
    }

    public Result Update(string? description, bool? isActive)
    {
        if (description is not null)
            Description = description.Trim();

        if (isActive.HasValue)
            IsActive = isActive.Value;

        return Result.Success();
    }

    public void AddUserRole(UserRole userRole) => _userRoles = [.. _userRoles, userRole];
    public void RemoveUserRole(UserRole userRole) => _userRoles = _userRoles.Where(ur => ur != userRole).ToArray();

    public void AddPermission(RolePermission rolePermission) => _rolePermissions = [.. _rolePermissions, rolePermission];
    public void RemovePermission(RolePermission rolePermission) => _rolePermissions = _rolePermissions.Where(rp => rp != rolePermission).ToArray();
}
