using Appointments.Domain.Entities;
using Shared.Application.Abstractions;

namespace Appointments.Application.Abstractions;
public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken = default);
    Task<Appointment?> GetBySlotIdAsync(Guid slotId, CancellationToken cancellationToken = default);
    Task<Appointment?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
}
