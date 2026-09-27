namespace MyApp.Scheduling.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Scheduling.Application.Abstractions;
using MyApp.Scheduling.Domain.Entities;
using MyApp.Scheduling.Infrastructure.Persistence.DbContext;

internal sealed class DoctorScheduleRepository : IDoctorScheduleRepository
{
    private readonly SchedulingDbContext _dbContext;

    public DoctorScheduleRepository(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DoctorSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorSchedules.AsNoTracking().FirstOrDefaultAsync(ds => ds.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DoctorSchedule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorSchedules.AsNoTracking().Where(ds => ds.IsActive).OrderBy(ds => ds.DoctorHospitalId).ThenBy(ds => ds.DayOfWeek).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DoctorSchedule>> GetByDoctorHospitalIdAsync(Guid doctorHospitalId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorSchedules.AsNoTracking().Where(ds => ds.DoctorHospitalId == doctorHospitalId && ds.IsActive).OrderBy(ds => ds.DayOfWeek).ToListAsync(cancellationToken);
    }

    public async Task<DoctorSchedule?> GetByDoctorHospitalAndDayAsync(Guid doctorHospitalId, int dayOfWeek, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorSchedules.AsNoTracking().FirstOrDefaultAsync(ds => ds.DoctorHospitalId == doctorHospitalId && ds.DayOfWeek == dayOfWeek && ds.IsActive, cancellationToken);
    }

    public async Task AddAsync(DoctorSchedule entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.DoctorSchedules.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(DoctorSchedule entity, CancellationToken cancellationToken = default)
    {
        _dbContext.DoctorSchedules.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(DoctorSchedule entity, CancellationToken cancellationToken = default)
    {
        _dbContext.DoctorSchedules.Remove(entity);
        await Task.CompletedTask;
    }
}
