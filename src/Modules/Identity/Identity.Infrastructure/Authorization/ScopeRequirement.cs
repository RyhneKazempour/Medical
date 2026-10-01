namespace MyApp.Identity.Infrastructure.Authorization;

using MyApp.Identity.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;

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