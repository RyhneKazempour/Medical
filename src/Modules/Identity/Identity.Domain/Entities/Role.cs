namespace MyApp.Identity.Domain.Entities;

using MyApp.Shared.Domain;

public sealed class Role : AuditableActivatableEntity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? ContextType { get; private set; }
    public Guid? ContextId { get; private set; }

    private UserRole[] _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

    private RolePermission[] _rolePermissions = [];
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

    private Role() { }

    private Role(Guid id, string name, string? description, string? contextType, Guid? contextId)
        : base(id)
    {
        Name = name;
        Description = description;
        ContextType = contextType;
        ContextId = contextId;
        IsActive = true;
        IsDeleted = false;
    }

    public static Result<Role> Create(string name, string? description, string? contextType, Guid? contextId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Role>.Failure(new Error("Role.NameRequired", "Role name is required."));

        if (contextType is not null && contextId is null)
            return Result<Role>.Failure(new Error("Role.ContextIdRequired", "Context ID is required when context type is specified."));

        if (contextType is null && contextId is not null)
            return Result<Role>.Failure(new Error("Role.ContextTypeRequired", "Context type is required when context ID is specified."));

        var role = new Role(Guid.NewGuid(), name.Trim(), description?.Trim(), contextType, contextId);
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
