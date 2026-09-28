namespace MyApp.Identity.Domain.Exceptions;

using MyApp.Shared.Domain.Exceptions;

public sealed class UserNotFoundException : DomainException
{
    public UserNotFoundException(Guid userId)
        : base($"User with ID '{userId}' was not found.")
    {
        UserId = userId;
    }

    public UserNotFoundException(string email)
        : base($"User with email '{email}' was not found.")
    {
        Email = email;
    }

    public Guid? UserId { get; }
    public string? Email { get; }
}

public sealed class UserEmailExistsException : DomainException
{
    public UserEmailExistsException(string email)
        : base($"User with email '{email}' already exists.")
    {
        Email = email;
    }

    public string Email { get; }
}

public sealed class UserMobileExistsException : DomainException
{
    public UserMobileExistsException(string mobile)
        : base($"User with mobile '{mobile}' already exists.")
    {
        Mobile = mobile;
    }

    public string Mobile { get; }
}

public sealed class RoleNotFoundException : DomainException
{
    public RoleNotFoundException(Guid roleId)
        : base($"Role with ID '{roleId}' was not found.")
    {
        RoleId = roleId;
    }

    public Guid RoleId { get; }
}

public sealed class RoleNameExistsException : DomainException
{
    public RoleNameExistsException(string name)
        : base($"Role with name '{name}' already exists.")
    {
        Name = name;
    }

    public string Name { get; }
}

public sealed class PermissionNotFoundException : DomainException
{
    public PermissionNotFoundException(Guid permissionId)
        : base($"Permission with ID '{permissionId}' was not found.")
    {
        PermissionId = permissionId;
    }

    public Guid PermissionId { get; }
}

public sealed class RolePermissionAlreadyExistsException : DomainException
{
    public RolePermissionAlreadyExistsException(Guid roleId, Guid permissionId)
        : base($"Role '{roleId}' already has permission '{permissionId}'.")
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public Guid RoleId { get; }
    public Guid PermissionId { get; }
}

public sealed class UserRoleAlreadyExistsException : DomainException
{
    public UserRoleAlreadyExistsException(Guid userId, Guid roleId, MyApp.Identity.Domain.ValueObjects.ScopeType scopeType, Guid scopeId)
        : base($"User '{userId}' already has role '{roleId}' in scope '{scopeType}' with ID '{scopeId}'.")
    {
        UserId = userId;
        RoleId = roleId;
        ScopeType = scopeType;
        ScopeId = scopeId;
    }

    public Guid UserId { get; }
    public Guid RoleId { get; }
    public MyApp.Identity.Domain.ValueObjects.ScopeType ScopeType { get; }
    public Guid ScopeId { get; }
}

public sealed class UserRoleConcurrencyException : DomainException
{
    public UserRoleConcurrencyException(Guid userRoleId)
        : base($"UserRole with ID '{userRoleId}' was modified by another process.")
    {
        UserRoleId = userRoleId;
    }

    public Guid UserRoleId { get; }
}
