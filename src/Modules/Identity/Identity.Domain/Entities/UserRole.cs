namespace MyApp.Identity.Domain.Entities;

using MyApp.Shared.Domain;

public sealed class UserRole : AuditableEntity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public string? ScopeType { get; private set; }
    public Guid? ScopeId { get; private set; }

    public User? User { get; private set; }
    public Role? Role { get; private set; }

    private UserRole() { }

    private UserRole(Guid id, Guid userId, Guid roleId, string? scopeType, Guid? scopeId)
        : base(id)
    {
        UserId = userId;
        RoleId = roleId;
        ScopeType = scopeType;
        ScopeId = scopeId;
        IsDeleted = false;
    }

    public static Result<UserRole> Create(Guid userId, Guid roleId, string? scopeType, Guid? scopeId)
    {
        if (userId == Guid.Empty)
            return Result<UserRole>.Failure(new Error("UserRole.UserIdRequired", "User ID is required."));

        if (roleId == Guid.Empty)
            return Result<UserRole>.Failure(new Error("UserRole.RoleIdRequired", "Role ID is required."));

        if (scopeType is not null && scopeId is null)
            return Result<UserRole>.Failure(new Error("UserRole.ScopeIdRequired", "Scope ID is required when scope type is specified."));

        if (scopeType is null && scopeId is not null)
            return Result<UserRole>.Failure(new Error("UserRole.ScopeTypeRequired", "Scope type is required when scope ID is specified."));

        var userRole = new UserRole(Guid.NewGuid(), userId, roleId, scopeType, scopeId);
        return Result<UserRole>.Success(userRole);
    }
}
