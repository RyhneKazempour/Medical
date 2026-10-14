using Identity.Domain.ValueObjects;

namespace Identity.Application.Authentication;

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
    ScopeType ScopeType,
    Guid ScopeId);