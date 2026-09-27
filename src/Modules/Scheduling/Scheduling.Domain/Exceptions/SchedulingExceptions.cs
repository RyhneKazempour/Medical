namespace MyApp.Scheduling.Domain.Exceptions;

using MyApp.Scheduling.Domain.Entities;
using MyApp.Shared.Domain.Exceptions;

public sealed class DoctorScheduleNotFoundException : DomainException
{
    public DoctorScheduleNotFoundException(int scheduleId)
        : base($"Doctor schedule with ID '{scheduleId}' was not found.")
    {
        ScheduleId = scheduleId;
    }

    public int ScheduleId { get; }
}

public sealed class DoctorScheduleExceptionNotFoundException : DomainException
{
    public DoctorScheduleExceptionNotFoundException(int exceptionId)
        : base($"Doctor schedule exception with ID '{exceptionId}' was not found.")
    {
        ExceptionId = exceptionId;
    }

    public int ExceptionId { get; }
}

public sealed class AppointmentSlotNotFoundException : DomainException
{
    public AppointmentSlotNotFoundException(int slotId)
        : base($"Appointment slot with ID '{slotId}' was not found.")
    {
        SlotId = slotId;
    }

    public int SlotId { get; }
}

public sealed class ScheduleConflictException : DomainException
{
    public ScheduleConflictException(int doctorHospitalId, int dayOfWeek)
        : base($"A schedule already exists for doctor hospital '{doctorHospitalId}' on day '{dayOfWeek}'.")
    {
        DoctorHospitalId = doctorHospitalId;
        DayOfWeek = dayOfWeek;
    }

    public int DoctorHospitalId { get; }
    public int DayOfWeek { get; }
}

public sealed class SlotNotAvailableException : DomainException
{
    public SlotNotAvailableException(int slotId, AppointmentSlotStatus currentStatus)
        : base($"Appointment slot '{slotId}' is not available (current status: {currentStatus}).")
    {
        SlotId = slotId;
        CurrentStatus = currentStatus;
    }

    public int SlotId { get; }
    public AppointmentSlotStatus CurrentStatus { get; }
}

public sealed class ExceptionAlreadyExistsException : DomainException
{
    public ExceptionAlreadyExistsException(int doctorScheduleId, DateOnly date)
        : base($"An exception already exists for schedule '{doctorScheduleId}' on date '{date}'.")
    {
        DoctorScheduleId = doctorScheduleId;
        Date = date;
    }

    public int DoctorScheduleId { get; }
    public DateOnly Date { get; }
}
