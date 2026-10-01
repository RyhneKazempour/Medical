namespace MyApp.Identity.Infrastructure.Authorization;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Domain.ValueObjects;

internal sealed class ScopeAuthorizationHandler : AuthorizationHandler<ScopeRequirement>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ScopeAuthorizationHandler(
        IUserRoleRepository userRoleRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _userRoleRepository = userRoleRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ScopeRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return;
        }

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return;
        }

        var scopeIdString = httpContext.Request.RouteValues[requirement.ScopeIdRouteParameter]?.ToString();
        if (scopeIdString is null || !Guid.TryParse(scopeIdString, out var scopeId))
        {
            return;
        }

        var userRoles = await _userRoleRepository.GetByUserIdIncludingRoleAsync(userId, CancellationToken.None);

        foreach (var userRole in userRoles)
        {
            if (userRole.Role is null)
            {
                continue;
            }

            var roleMatches = requirement.AllowedRoles.Count == 0
                || requirement.AllowedRoles.Any(r => string.Equals(r, userRole.Role.Name, StringComparison.OrdinalIgnoreCase));

            if (!roleMatches)
            {
                continue;
            }

            if (userRole.ScopeType == requirement.ScopeType && userRole.ScopeId == scopeId)
            {
                context.Succeed(requirement);
                return;
            }

            if (requirement.AllowGlobal && userRole.ScopeType == ScopeType.Global)
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}