namespace MyApp.Medical.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Medical.Application.Abstractions;
using MyApp.Medical.Domain.Entities;
using MyApp.Medical.Infrastructure.Persistence.DbContext;

internal sealed class DoctorRepository : IDoctorRepository
{
    private readonly MedicalDbContext _dbContext;

    public DoctorRepository(MedicalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Doctor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Doctors.AsNoTracking().Where(d => d.IsActive).OrderBy(d => d.LicenseNumber).ToListAsync(cancellationToken);
    }

    public async Task<Doctor?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.UserId == userId, cancellationToken);
    }

    public async Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Doctor>> GetBySpecializationIdAsync(Guid specializationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Doctors.AsNoTracking()
            .Where(d => d.DoctorSpecializations.Any(ds => ds.SpecializationId == specializationId && ds.IsActive) && d.IsActive)
            .OrderBy(d => d.LicenseNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Doctor entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Doctors.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Doctor entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Doctors.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Doctor entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Doctors.Remove(entity);
        await Task.CompletedTask;
    }
}
