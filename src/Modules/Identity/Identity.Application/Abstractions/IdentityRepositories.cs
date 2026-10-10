namespace MyApp.Identity.Application.Abstractions;

using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Application.Abstractions;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<RolePermission?> GetPermissionOfRole(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);
    Task AddPermissionToRole(RolePermission entity, CancellationToken cancellationToken = default);
}

public interface IPermissionRepository : IRepository<Permission>
{
    Task<Permission?> GetByResourceActionAsync(string resource, string action, CancellationToken cancellationToken = default);
}

public interface IUserRoleRepository : IRepository<UserRole>
{
    Task<IReadOnlyList<UserRole>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserRole>> GetByUserIdIncludingRoleAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserRole>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<UserRole?> GetByUserRoleScopeAsync(Guid userId, Guid roleId, MyApp.Identity.Domain.ValueObjects.ScopeType scopeType, Guid scopeId, CancellationToken cancellationToken = default);
    Task<UserRole?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RevokeAllUserTokensAsync(Guid userId, string? revokedByIpAddress, Guid? replacedByTokenId, CancellationToken cancellationToken = default);
}
