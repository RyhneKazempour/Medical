namespace Identity.Infrastructure.Authorization;

using Microsoft.AspNetCore.Authorization;
using Identity.Domain.ValueObjects;

public sealed class ScopeRequirement : IAuthorizationRequirement
{
    public ScopeRequirement(ScopeType scopeType, string scopeIdRouteParameter, IEnumerable<string>? allowedRoles = null, bool allowGlobal = true)
    {
        ScopeType = scopeType;
        ScopeIdRouteParameter = scopeIdRouteParameter;
        AllowedRoles = allowedRoles?.ToArray() ?? Array.Empty<string>();
        AllowGlobal = allowGlobal;
    }

    public ScopeType ScopeType { get; }
    public string ScopeIdRouteParameter { get; }
    public IReadOnlyCollection<string> AllowedRoles { get; }
    public bool AllowGlobal { get; }
}