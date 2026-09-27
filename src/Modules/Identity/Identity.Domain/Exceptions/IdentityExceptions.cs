namespace MyApp.Identity.Domain.Exceptions;

using MyApp.Shared.Domain.Exceptions;

public sealed class UserNotFoundException : DomainException
{
    public UserNotFoundException(int userId)
        : base($"User with ID '{userId}' was not found.")
    {
        UserId = userId;
    }

    public UserNotFoundException(string email)
        : base($"User with email '{email}' was not found.")
    {
        Email = email;
    }

    public int? UserId { get; }
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

public sealed class RoleNotFoundException : DomainException
{
    public RoleNotFoundException(int roleId)
        : base($"Role with ID '{roleId}' was not found.")
    {
        RoleId = roleId;
    }

    public int RoleId { get; }
}

public sealed class PermissionNotFoundException : DomainException
{
    public PermissionNotFoundException(int permissionId)
        : base($"Permission with ID '{permissionId}' was not found.")
    {
        PermissionId = permissionId;
    }

    public int PermissionId { get; }
}

public sealed class RolePermissionAlreadyExistsException : DomainException
{
    public RolePermissionAlreadyExistsException(int roleId, int permissionId)
        : base($"Role '{roleId}' already has permission '{permissionId}'.")
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public int RoleId { get; }
    public int PermissionId { get; }
}
