namespace MyApp.Scheduling.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Scheduling.Application.Abstractions;
using MyApp.Scheduling.Domain.Entities;
using MyApp.Scheduling.Infrastructure.Persistence.DbContext;

internal sealed class AppointmentSlotRepository : IAppointmentSlotRepository
{
    private readonly SchedulingDbContext _dbContext;

    public AppointmentSlotRepository(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppointmentSlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AppointmentSlots.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<AppointmentSlot>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.AppointmentSlots.AsNoTracking().OrderBy(a => a.Date).ThenBy(a => a.StartTime).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AppointmentSlot>> GetByDoctorScheduleIdAsync(Guid doctorScheduleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AppointmentSlots.AsNoTracking().Where(a => a.DoctorScheduleId == doctorScheduleId).OrderBy(a => a.Date).ThenBy(a => a.StartTime).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AppointmentSlot>> GetAvailableByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AppointmentSlots.AsNoTracking()
            .Where(a => a.Date == date && a.Status == AppointmentSlotStatus.Available)
            .OrderBy(a => a.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AppointmentSlot>> GetByDateRangeAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AppointmentSlots.AsNoTracking()
            .Where(a => a.Date >= fromDate && a.Date <= toDate)
            .OrderBy(a => a.Date).ThenBy(a => a.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<AppointmentSlot?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AppointmentSlots.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task AddAsync(AppointmentSlot entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.AppointmentSlots.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(AppointmentSlot entity, CancellationToken cancellationToken = default)
    {
        _dbContext.AppointmentSlots.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(AppointmentSlot entity, CancellationToken cancellationToken = default)
    {
        _dbContext.AppointmentSlots.Remove(entity);
        await Task.CompletedTask;
    }
}
