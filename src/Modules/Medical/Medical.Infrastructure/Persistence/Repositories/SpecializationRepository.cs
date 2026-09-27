namespace MyApp.Medical.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Medical.Application.Abstractions;
using MyApp.Medical.Domain.Entities;
using MyApp.Medical.Infrastructure.Persistence.DbContext;

internal sealed class SpecializationRepository : ISpecializationRepository
{
    private readonly MedicalDbContext _dbContext;

    public SpecializationRepository(MedicalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Specialization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Specializations.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Specialization>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Specializations.AsNoTracking().Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync(cancellationToken);
    }

    public async Task<Specialization?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Specializations.AsNoTracking().FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
    }

    public async Task AddAsync(Specialization entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Specializations.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Specialization entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Specializations.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Specialization entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Specializations.Remove(entity);
        await Task.CompletedTask;
    }
}
