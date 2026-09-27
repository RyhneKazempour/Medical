namespace MyApp.Medical.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Medical.Application.Abstractions;
using MyApp.Medical.Domain.Entities;
using MyApp.Medical.Infrastructure.Persistence.DbContext;

internal sealed class HospitalRepository : IHospitalRepository
{
    private readonly MedicalDbContext _dbContext;

    public HospitalRepository(MedicalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Hospital?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Hospitals.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Hospital>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Hospitals.AsNoTracking().Where(h => h.IsActive).OrderBy(h => h.Name).ToListAsync(cancellationToken);
    }

    public async Task<Hospital?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Hospitals.AsNoTracking().FirstOrDefaultAsync(h => h.Name == name, cancellationToken);
    }

    public async Task AddAsync(Hospital entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Hospitals.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Hospital entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Hospitals.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Hospital entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Hospitals.Remove(entity);
        await Task.CompletedTask;
    }
}
