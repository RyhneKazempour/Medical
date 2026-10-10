namespace MyApp.Identity.Application.Authentication;

public sealed record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? Phone,
    string? Mobile,
    bool IsActive,
    bool IsDeleted,
    IReadOnlyList<UserRoleResponse> UserRoles);

public sealed record UserRoleResponse(
    string RoleName,
    MyApp.Identity.Domain.ValueObjects.ScopeType ScopeType,
    Guid ScopeId);