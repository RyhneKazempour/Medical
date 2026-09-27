namespace MyApp.Scheduling.Application.Abstractions;

using MyApp.Scheduling.Domain.Entities;
using MyApp.Shared.Application.Abstractions;

public interface IDoctorScheduleRepository : IRepository<DoctorSchedule>
{
    Task<IReadOnlyList<DoctorSchedule>> GetByDoctorHospitalIdAsync(Guid doctorHospitalId, CancellationToken cancellationToken = default);
    Task<DoctorSchedule?> GetByDoctorHospitalAndDayAsync(Guid doctorHospitalId, int dayOfWeek, CancellationToken cancellationToken = default);
}

public interface IDoctorScheduleExceptionRepository : IRepository<DoctorScheduleException>
{
    Task<IReadOnlyList<DoctorScheduleException>> GetByDoctorScheduleIdAsync(Guid doctorScheduleId, CancellationToken cancellationToken = default);
    Task<DoctorScheduleException?> GetByDoctorScheduleAndDateAsync(Guid doctorScheduleId, DateOnly date, CancellationToken cancellationToken = default);
}

public interface IAppointmentSlotRepository : IRepository<AppointmentSlot>
{
    Task<IReadOnlyList<AppointmentSlot>> GetByDoctorScheduleIdAsync(Guid doctorScheduleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppointmentSlot>> GetAvailableByDateAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppointmentSlot>> GetByDateRangeAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default);
    Task<AppointmentSlot?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
}
