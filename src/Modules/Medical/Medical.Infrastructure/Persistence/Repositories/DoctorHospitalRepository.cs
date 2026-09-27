namespace MyApp.Medical.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Medical.Application.Abstractions;
using MyApp.Medical.Domain.Entities;
using MyApp.Medical.Infrastructure.Persistence.DbContext;

internal sealed class DoctorHospitalRepository : IDoctorHospitalRepository
{
    private readonly MedicalDbContext _dbContext;

    public DoctorHospitalRepository(MedicalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DoctorHospital?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorHospitals.AsNoTracking().FirstOrDefaultAsync(dh => dh.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DoctorHospital>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorHospitals.AsNoTracking().Where(dh => dh.IsActive).OrderBy(dh => dh.DoctorSpecializationId).ThenBy(dh => dh.ClinicId).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DoctorHospital>> GetByDoctorSpecializationIdAsync(Guid doctorSpecializationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorHospitals.AsNoTracking().Where(dh => dh.DoctorSpecializationId == doctorSpecializationId && dh.IsActive).OrderBy(dh => dh.ClinicId).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DoctorHospital>> GetByClinicIdAsync(Guid clinicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorHospitals.AsNoTracking().Where(dh => dh.ClinicId == clinicId && dh.IsActive).OrderBy(dh => dh.DoctorSpecializationId).ToListAsync(cancellationToken);
    }

    public async Task<DoctorHospital?> GetByDoctorSpecializationAndClinicAsync(Guid doctorSpecializationId, Guid clinicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorHospitals.AsNoTracking().FirstOrDefaultAsync(dh => dh.DoctorSpecializationId == doctorSpecializationId && dh.ClinicId == clinicId && dh.IsActive, cancellationToken);
    }

    public async Task AddAsync(DoctorHospital entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.DoctorHospitals.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(DoctorHospital entity, CancellationToken cancellationToken = default)
    {
        _dbContext.DoctorHospitals.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(DoctorHospital entity, CancellationToken cancellationToken = default)
    {
        _dbContext.DoctorHospitals.Remove(entity);
        await Task.CompletedTask;
    }
}
