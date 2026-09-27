namespace MyApp.Identity.Domain.Entities;

using MyApp.Shared.Domain;

public sealed class Permission : AuditableEntity
{
    public string Resource { get; private set; } = null!;
    public string Action { get; private set; } = null!;
    public string? Description { get; private set; }

    private RolePermission[] _rolePermissions = [];
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

    private Permission() { }

    private Permission(Guid id, string resource, string action, string? description)
        : base(id)
    {
        Resource = resource;
        Action = action;
        Description = description;
        IsDeleted = false;
    }

    public static Result<Permission> Create(string resource, string action, string? description)
    {
        if (string.IsNullOrWhiteSpace(resource))
            return Result<Permission>.Failure(new Error("Permission.ResourceRequired", "Resource is required."));

        if (string.IsNullOrWhiteSpace(action))
            return Result<Permission>.Failure(new Error("Permission.ActionRequired", "Action is required."));

        var permission = new Permission(Guid.NewGuid(), resource.Trim().ToLowerInvariant(), action.Trim().ToLowerInvariant(), description?.Trim());
        return Result<Permission>.Success(permission);
    }

    public void Update(string? description)
    {
        if (description is not null)
            Description = description.Trim();
    }
}
