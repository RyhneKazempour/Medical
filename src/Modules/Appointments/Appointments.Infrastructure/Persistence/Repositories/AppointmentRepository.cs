namespace MyApp.Appointments.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using MyApp.Appointments.Application.Abstractions;
using MyApp.Appointments.Domain.Entities;
using MyApp.Appointments.Infrastructure.Persistence.DbContext;

internal sealed class AppointmentRepository : IAppointmentRepository
{
    private readonly AppointmentsDbContext _dbContext;

    public AppointmentRepository(AppointmentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Appointments.AsNoTracking().OrderByDescending(a => a.ReservedAt).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Appointments.AsNoTracking().Where(a => a.PatientId == patientId).OrderByDescending(a => a.ReservedAt).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Appointments.AsNoTracking().Where(a => a.Status == status).OrderByDescending(a => a.ReservedAt).ToListAsync(cancellationToken);
    }

    public async Task<Appointment?> GetBySlotIdAsync(Guid slotId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.SlotId == slotId, cancellationToken);
    }

    public async Task<Appointment?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Appointments.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task AddAsync(Appointment entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Appointments.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Appointment entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Appointments.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Appointment entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Appointments.Remove(entity);
        await Task.CompletedTask;
    }
}
