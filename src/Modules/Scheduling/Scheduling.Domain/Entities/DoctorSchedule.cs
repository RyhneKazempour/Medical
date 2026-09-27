namespace MyApp.Scheduling.Domain.Entities;

using MyApp.Shared.Domain;

public enum ScheduleExceptionType
{
    Cancelled = 1,
    Rescheduled = 2,
    Extended = 3
}

public sealed class DoctorSchedule : AuditableActivatableEntity
{
    public Guid DoctorHospitalId { get; private set; }
    public int DayOfWeek { get; private set; } // 0 = Sunday, 6 = Saturday
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public int SlotDurationMinutes { get; private set; }

    private DoctorScheduleException[] _exceptions = [];
    public IReadOnlyCollection<DoctorScheduleException> Exceptions => _exceptions;

    private AppointmentSlot[] _appointmentSlots = [];
    public IReadOnlyCollection<AppointmentSlot> AppointmentSlots => _appointmentSlots;

    private DoctorSchedule() { }

    private DoctorSchedule(Guid id, Guid doctorHospitalId, int dayOfWeek, TimeOnly startTime, TimeOnly endTime, int slotDurationMinutes)
        : base(id)
    {
        DoctorHospitalId = doctorHospitalId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        SlotDurationMinutes = slotDurationMinutes;
        IsActive = true;
        IsDeleted = false;
    }

    public static Result<DoctorSchedule> Create(Guid doctorHospitalId, int dayOfWeek, TimeOnly startTime, TimeOnly endTime, int slotDurationMinutes)
    {
        if (doctorHospitalId == Guid.Empty)
            return Result<DoctorSchedule>.Failure(new Error("DoctorSchedule.DoctorHospitalIdRequired", "Doctor hospital ID is required."));

        if (dayOfWeek < 0 || dayOfWeek > 6)
            return Result<DoctorSchedule>.Failure(new Error("DoctorSchedule.InvalidDayOfWeek", "Day of week must be between 0 (Sunday) and 6 (Saturday)."));

        if (startTime >= endTime)
            return Result<DoctorSchedule>.Failure(new Error("DoctorSchedule.InvalidTimeRange", "Start time must be before end time."));

        if (slotDurationMinutes <= 0)
            return Result<DoctorSchedule>.Failure(new Error("DoctorSchedule.InvalidSlotDuration", "Slot duration must be positive."));

        var schedule = new DoctorSchedule(Guid.NewGuid(), doctorHospitalId, dayOfWeek, startTime, endTime, slotDurationMinutes);
        return Result<DoctorSchedule>.Success(schedule);
    }

    public Result Update(TimeOnly? startTime, TimeOnly? endTime, int? slotDurationMinutes, bool? isActive)
    {
        if (startTime.HasValue && endTime.HasValue && startTime.Value >= endTime.Value)
            return Result.Failure(new Error("DoctorSchedule.InvalidTimeRange", "Start time must be before end time."));

        if (slotDurationMinutes.HasValue && slotDurationMinutes.Value <= 0)
            return Result.Failure(new Error("DoctorSchedule.InvalidSlotDuration", "Slot duration must be positive."));

        if (startTime.HasValue) StartTime = startTime.Value;
        if (endTime.HasValue) EndTime = endTime.Value;
        if (slotDurationMinutes.HasValue) SlotDurationMinutes = slotDurationMinutes.Value;
        if (isActive.HasValue) IsActive = isActive.Value;

        return Result.Success();
    }

    public Result AddException(DoctorScheduleException exception)
    {
        if (_exceptions.Any(e => e.ExceptionDate == exception.ExceptionDate && !e.IsDeleted))
            return Result.Failure(new Error("DoctorSchedule.ExceptionExists", "An exception already exists for this date."));

        _exceptions = [.. _exceptions, exception];
        return Result.Success();
    }

    public void RemoveException(DateOnly date)
    {
        _exceptions = _exceptions.Where(e => e.ExceptionDate != date).ToArray();
    }

    public void AddAppointmentSlot(AppointmentSlot slot) => _appointmentSlots = [.. _appointmentSlots, slot];
}
