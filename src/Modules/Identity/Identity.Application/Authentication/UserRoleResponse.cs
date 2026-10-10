namespace MyApp.Identity.Application.Authentication;

public sealed record UserRoleDto(
    Guid Id,
    MyApp.Identity.Domain.ValueObjects.ScopeType ScopeType,
    Guid ScopeId,
    string RoleName);