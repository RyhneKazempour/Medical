namespace MyApp.Identity.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Domain.Entities;
using MyApp.Identity.Infrastructure.Persistence.DbContext;

internal sealed class PermissionRepository : IPermissionRepository
{
    private readonly IdentityDbContext _dbContext;

    public PermissionRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions.AsNoTracking().OrderBy(p => p.Resource).ThenBy(p => p.Action).ToListAsync(cancellationToken);
    }

    public async Task<Permission?> GetByResourceActionAsync(string resource, string action, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Resource == resource.ToLowerInvariant() && p.Action == action.ToLowerInvariant(), cancellationToken);
    }

    public async Task AddAsync(Permission entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Permissions.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Permission entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Permissions.Update(entity);
        await Task.CompletedTask;
    }
}
