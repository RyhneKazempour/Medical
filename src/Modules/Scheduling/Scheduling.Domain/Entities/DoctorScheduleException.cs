namespace MyApp.Scheduling.Domain.Entities;

using MyApp.Shared.Domain;
using System.ComponentModel.DataAnnotations.Schema;

public sealed class DoctorScheduleException : AuditableEntity
{
    public Guid DoctorScheduleId { get; private set; }
    public DateOnly ExceptionDate { get; private set; }
    public ScheduleExceptionType ExceptionType { get; private set; }
    public TimeOnly? NewStartTime { get; private set; }
    public TimeOnly? NewEndTime { get; private set; }
    public string? Reason { get; private set; }

    [ForeignKey(nameof(DoctorScheduleId))]
    public DoctorSchedule? DoctorSchedule { get; private set; }

    private DoctorScheduleException() { }

    private DoctorScheduleException(Guid id, Guid doctorScheduleId, DateOnly exceptionDate, ScheduleExceptionType exceptionType,
        TimeOnly? newStartTime, TimeOnly? newEndTime, string? reason)
        : base(id)
    {
        DoctorScheduleId = doctorScheduleId;
        ExceptionDate = exceptionDate;
        ExceptionType = exceptionType;
        NewStartTime = newStartTime;
        NewEndTime = newEndTime;
        Reason = reason;
        IsDeleted = false;
    }

    public static Result<DoctorScheduleException> Create(Guid doctorScheduleId, DateOnly exceptionDate,
        ScheduleExceptionType exceptionType, TimeOnly? newStartTime, TimeOnly? newEndTime, string? reason)
    {
        if (doctorScheduleId == Guid.Empty)
            return Result<DoctorScheduleException>.Failure(new Error("DoctorScheduleException.DoctorScheduleIdRequired", "Doctor schedule ID is required."));

        if (exceptionType == ScheduleExceptionType.Rescheduled || exceptionType == ScheduleExceptionType.Extended)
        {
            if (!newStartTime.HasValue || !newEndTime.HasValue)
                return Result<DoctorScheduleException>.Failure(new Error("DoctorScheduleException.NewTimesRequired", "New start and end times are required for rescheduled/extended exceptions."));

            if (newStartTime.Value >= newEndTime.Value)
                return Result<DoctorScheduleException>.Failure(new Error("DoctorScheduleException.InvalidTimeRange", "New start time must be before new end time."));
        }

        var exception = new DoctorScheduleException(Guid.NewGuid(), doctorScheduleId, exceptionDate, exceptionType, newStartTime, newEndTime, reason?.Trim());
        return Result<DoctorScheduleException>.Success(exception);
    }

    public Result Update(ScheduleExceptionType? exceptionType, TimeOnly? newStartTime, TimeOnly? newEndTime, string? reason)
    {
        if (exceptionType.HasValue)
        {
            if ((exceptionType.Value == ScheduleExceptionType.Rescheduled || exceptionType.Value == ScheduleExceptionType.Extended) &&
                (!newStartTime.HasValue || !newEndTime.HasValue))
                return Result.Failure(new Error("DoctorScheduleException.NewTimesRequired", "New start and end times are required for rescheduled/extended exceptions."));

            if (newStartTime.HasValue && newEndTime.HasValue && newStartTime.Value >= newEndTime.Value)
                return Result.Failure(new Error("DoctorScheduleException.InvalidTimeRange", "New start time must be before new end time."));

            ExceptionType = exceptionType.Value;
        }

        if (newStartTime.HasValue) NewStartTime = newStartTime.Value;
        if (newEndTime.HasValue) NewEndTime = newEndTime.Value;
        if (reason is not null) Reason = reason.Trim();

        return Result.Success();
    }
}
