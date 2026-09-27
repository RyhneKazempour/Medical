namespace MyApp.Medical.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Medical.Application.Abstractions;
using MyApp.Medical.Domain.Entities;
using MyApp.Medical.Infrastructure.Persistence.DbContext;

internal sealed class DoctorSpecializationRepository : IDoctorSpecializationRepository
{
    private readonly MedicalDbContext _dbContext;

    public DoctorSpecializationRepository(MedicalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DoctorSpecialization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorSpecializations.AsNoTracking().FirstOrDefaultAsync(ds => ds.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DoctorSpecialization>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorSpecializations.AsNoTracking().Where(ds => ds.IsActive).OrderBy(ds => ds.DoctorId).ThenBy(ds => ds.SpecializationId).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DoctorSpecialization>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorSpecializations.AsNoTracking().Where(ds => ds.DoctorId == doctorId && ds.IsActive).OrderBy(ds => ds.SpecializationId).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DoctorSpecialization>> GetBySpecializationIdAsync(Guid specializationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorSpecializations.AsNoTracking().Where(ds => ds.SpecializationId == specializationId && ds.IsActive).OrderBy(ds => ds.DoctorId).ToListAsync(cancellationToken);
    }

    public async Task<DoctorSpecialization?> GetByDoctorAndSpecializationAsync(Guid doctorId, Guid specializationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorSpecializations.AsNoTracking().FirstOrDefaultAsync(ds => ds.DoctorId == doctorId && ds.SpecializationId == specializationId && ds.IsActive, cancellationToken);
    }

    public async Task AddAsync(DoctorSpecialization entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.DoctorSpecializations.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(DoctorSpecialization entity, CancellationToken cancellationToken = default)
    {
        _dbContext.DoctorSpecializations.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(DoctorSpecialization entity, CancellationToken cancellationToken = default)
    {
        _dbContext.DoctorSpecializations.Remove(entity);
        await Task.CompletedTask;
    }
}
