namespace Insurance.Infrastructure.Persistence.Repositories;

using Insurance.Application.Abstractions;
using Insurance.Domain.Entities;
using Insurance.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

internal sealed class InsuranceRepository : IInsuranceRepository
{
    private readonly InsuranceDbContext _dbContext;

    public InsuranceRepository(InsuranceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Insurance.Domain.Entities.InsuranceInfo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Insurances.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<InsuranceInfo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Insurances.AsNoTracking().Where(i => i.IsActive).OrderBy(i => i.Name).ToListAsync(cancellationToken);
    }

    public async Task<InsuranceInfo?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Insurances.AsNoTracking().FirstOrDefaultAsync(i => i.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task AddAsync(InsuranceInfo entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Insurances.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(InsuranceInfo entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Insurances.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(InsuranceInfo entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Insurances.Remove(entity);
        await Task.CompletedTask;
    }
}
