using Identity.Domain.ValueObjects;

namespace Identity.Application.Authentication;

public sealed record UserRoleDto(
    Guid Id,
    ScopeType ScopeType,
    Guid ScopeId,
    string RoleName);