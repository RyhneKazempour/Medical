namespace MyApp.Identity.Application.Abstractions;

using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Application.Abstractions;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetByContextAsync(string? contextType, Guid? contextId, CancellationToken cancellationToken = default);
}

public interface IPermissionRepository : IRepository<Permission>
{
    Task<Permission?> GetByResourceActionAsync(string resource, string action, CancellationToken cancellationToken = default);
}

public interface IUserRoleRepository : IRepository<UserRole>
{
    Task<IReadOnlyList<UserRole>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserRole>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<UserRole?> GetByUserAndRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}
