namespace MyApp.Identity.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Domain.Entities;
using MyApp.Identity.Infrastructure.Persistence.DbContext;

internal sealed class UserRoleRepository : IUserRoleRepository
{
    private readonly IdentityDbContext _dbContext;

    public UserRoleRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRoles.AsNoTracking().FirstOrDefaultAsync(ur => ur.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<UserRole>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRoles.AsNoTracking().OrderBy(ur => ur.UserId).ThenBy(ur => ur.RoleId).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserRole>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRoles.AsNoTracking().Where(ur => ur.UserId == userId).OrderBy(ur => ur.RoleId).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserRole>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRoles.AsNoTracking().Where(ur => ur.RoleId == roleId).OrderBy(ur => ur.UserId).ToListAsync(cancellationToken);
    }

    public async Task<UserRole?> GetByUserAndRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRoles.AsNoTracking().FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }

    public async Task AddAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.UserRoles.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        _dbContext.UserRoles.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        _dbContext.UserRoles.Remove(entity);
        await Task.CompletedTask;
    }
}
