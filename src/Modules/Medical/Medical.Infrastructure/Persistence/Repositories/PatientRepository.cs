namespace MyApp.Medical.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Medical.Application.Abstractions;
using MyApp.Medical.Domain.Entities;
using MyApp.Medical.Infrastructure.Persistence.DbContext;

internal sealed class PatientRepository : IPatientRepository
{
    private readonly MedicalDbContext _dbContext;

    public PatientRepository(MedicalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients.AsNoTracking().OrderBy(p => p.Id).ToListAsync(cancellationToken);
    }

    public async Task<Patient?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(Patient entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Patients.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Patient entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Patients.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Patient entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Patients.Remove(entity);
        await Task.CompletedTask;
    }
}
