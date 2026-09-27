namespace MyApp.Appointments.Application.Abstractions;

using MyApp.Appointments.Domain.Entities;
using MyApp.Shared.Application.Abstractions;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken = default);
    Task<Appointment?> GetBySlotIdAsync(Guid slotId, CancellationToken cancellationToken = default);
    Task<Appointment?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
}
