namespace MyApp.Scheduling.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Scheduling.Application.Abstractions;
using MyApp.Scheduling.Domain.Entities;
using MyApp.Scheduling.Infrastructure.Persistence.DbContext;

internal sealed class DoctorScheduleExceptionRepository : IDoctorScheduleExceptionRepository
{
    private readonly SchedulingDbContext _dbContext;

    public DoctorScheduleExceptionRepository(SchedulingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DoctorScheduleException?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorScheduleExceptions.AsNoTracking().FirstOrDefaultAsync(dse => dse.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DoctorScheduleException>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorScheduleExceptions.AsNoTracking().OrderBy(dse => dse.DoctorScheduleId).ThenBy(dse => dse.ExceptionDate).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DoctorScheduleException>> GetByDoctorScheduleIdAsync(Guid doctorScheduleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorScheduleExceptions.AsNoTracking().Where(dse => dse.DoctorScheduleId == doctorScheduleId).OrderBy(dse => dse.ExceptionDate).ToListAsync(cancellationToken);
    }

    public async Task<DoctorScheduleException?> GetByDoctorScheduleAndDateAsync(Guid doctorScheduleId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorScheduleExceptions.AsNoTracking().FirstOrDefaultAsync(dse => dse.DoctorScheduleId == doctorScheduleId && dse.ExceptionDate == date, cancellationToken);
    }

    public async Task AddAsync(DoctorScheduleException entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.DoctorScheduleExceptions.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(DoctorScheduleException entity, CancellationToken cancellationToken = default)
    {
        _dbContext.DoctorScheduleExceptions.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(DoctorScheduleException entity, CancellationToken cancellationToken = default)
    {
        _dbContext.DoctorScheduleExceptions.Remove(entity);
        await Task.CompletedTask;
    }
}
