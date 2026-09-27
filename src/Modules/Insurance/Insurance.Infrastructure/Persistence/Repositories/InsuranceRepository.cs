namespace MyApp.Insurance.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Insurance.Application.Abstractions;
using MyApp.Insurance.Domain.Entities;
using MyApp.Insurance.Infrastructure.Persistence.DbContext;

internal sealed class InsuranceRepository : IInsuranceRepository
{
    private readonly InsuranceDbContext _dbContext;

    public InsuranceRepository(InsuranceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Insurance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Insurances.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Insurance>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Insurances.AsNoTracking().Where(i => i.IsActive).OrderBy(i => i.Name).ToListAsync(cancellationToken);
    }

    public async Task<Insurance?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Insurances.AsNoTracking().FirstOrDefaultAsync(i => i.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task AddAsync(Insurance entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Insurances.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Insurance entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Insurances.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Insurance entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Insurances.Remove(entity);
        await Task.CompletedTask;
    }
}
