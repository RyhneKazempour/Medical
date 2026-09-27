namespace MyApp.Scheduling.Domain.Entities;

using MyApp.Shared.Domain;

public enum AppointmentSlotStatus
{
    Available = 1,
    Reserved = 2,
    Booked = 3,
    Blocked = 4
}

public sealed class AppointmentSlot : AuditableEntity
{
    public Guid DoctorScheduleId { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public AppointmentSlotStatus Status { get; private set; }

    public DoctorSchedule? DoctorSchedule { get; private set; }

    private AppointmentSlot() { }

    private AppointmentSlot(Guid id, Guid doctorScheduleId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        : base(id)
    {
        DoctorScheduleId = doctorScheduleId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        Status = AppointmentSlotStatus.Available;
        IsDeleted = false;
    }

    public static Result<AppointmentSlot> Create(Guid doctorScheduleId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        if (doctorScheduleId == Guid.Empty)
            return Result<AppointmentSlot>.Failure(new Error("AppointmentSlot.DoctorScheduleIdRequired", "Doctor schedule ID is required."));

        if (startTime >= endTime)
            return Result<AppointmentSlot>.Failure(new Error("AppointmentSlot.InvalidTimeRange", "Start time must be before end time."));

        var slot = new AppointmentSlot(Guid.NewGuid(), doctorScheduleId, date, startTime, endTime);
        return Result<AppointmentSlot>.Success(slot);
    }

    public Result Reserve()
    {
        if (Status != AppointmentSlotStatus.Available)
            return Result.Failure(new Error("AppointmentSlot.NotAvailable", "Slot is not available for reservation."));

        Status = AppointmentSlotStatus.Reserved;
        return Result.Success();
    }

    public Result Book()
    {
        if (Status != AppointmentSlotStatus.Reserved)
            return Result.Failure(new Error("AppointmentSlot.NotReserved", "Slot must be reserved before booking."));

        Status = AppointmentSlotStatus.Booked;
        return Result.Success();
    }

    public Result Release()
    {
        if (Status == AppointmentSlotStatus.Booked)
            return Result.Failure(new Error("AppointmentSlot.AlreadyBooked", "Cannot release a booked slot."));

        Status = AppointmentSlotStatus.Available;
        return Result.Success();
    }

    public Result Block()
    {
        if (Status == AppointmentSlotStatus.Booked)
            return Result.Failure(new Error("AppointmentSlot.AlreadyBooked", "Cannot block a booked slot."));

        Status = AppointmentSlotStatus.Blocked;
        return Result.Success();
    }

    public Result Unblock()
    {
        if (Status != AppointmentSlotStatus.Blocked)
            return Result.Failure(new Error("AppointmentSlot.NotBlocked", "Slot is not blocked."));

        Status = AppointmentSlotStatus.Available;
        return Result.Success();
    }
}
