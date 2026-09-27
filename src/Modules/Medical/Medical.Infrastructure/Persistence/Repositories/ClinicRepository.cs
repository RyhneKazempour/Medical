namespace MyApp.Medical.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Medical.Application.Abstractions;
using MyApp.Medical.Domain.Entities;
using MyApp.Medical.Infrastructure.Persistence.DbContext;

internal sealed class ClinicRepository : IClinicRepository
{
    private readonly MedicalDbContext _dbContext;

    public ClinicRepository(MedicalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Clinic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Clinics.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Clinic>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Clinics.AsNoTracking().Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Clinic>> GetByHospitalIdAsync(Guid hospitalId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Clinics.AsNoTracking().Where(c => c.HospitalId == hospitalId && c.IsActive).OrderBy(c => c.Name).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Clinic entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Clinics.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Clinic entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Clinics.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Clinic entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Clinics.Remove(entity);
        await Task.CompletedTask;
    }
}
