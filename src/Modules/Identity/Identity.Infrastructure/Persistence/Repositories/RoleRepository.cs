namespace MyApp.Identity.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Domain.Entities;
using MyApp.Identity.Infrastructure.Persistence.DbContext;

internal sealed class RoleRepository : IRoleRepository
{
    private readonly IdentityDbContext _dbContext;

    public RoleRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles.AsNoTracking().Where(r => r.IsActive).OrderBy(r => r.Name).ToListAsync(cancellationToken);
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }

    public async Task AddAsync(Role entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Roles.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Role entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Roles.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Role entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Roles.Remove(entity);
        await Task.CompletedTask;
    }
}
