namespace MyApp.Identity.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Domain.Entities;
using MyApp.Identity.Infrastructure.Persistence.DbContext;

internal sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IdentityDbContext _dbContext;

    public RefreshTokenRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefreshToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(rt => rt.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens.AsNoTracking().OrderBy(rt => rt.UserId).ThenBy(rt => rt.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens
            .AsNoTracking()
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTimeOffset.UtcNow)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, string? revokedByIpAddress, Guid? replacedByTokenId, CancellationToken cancellationToken = default)
    {
        var tokens = await _dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.Revoke(revokedByIpAddress, replacedByTokenId);
        }

        await Task.CompletedTask;
    }

    public async Task AddAsync(RefreshToken entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.RefreshTokens.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(RefreshToken entity, CancellationToken cancellationToken = default)
    {
        _dbContext.RefreshTokens.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(RefreshToken entity, CancellationToken cancellationToken = default)
    {
        _dbContext.RefreshTokens.Remove(entity);
        await Task.CompletedTask;
    }
}