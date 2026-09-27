namespace MyApp.Appointments.Domain.Exceptions;

using MyApp.Appointments.Domain.Entities;
using MyApp.Shared.Domain.Exceptions;

public sealed class AppointmentNotFoundException : DomainException
{
    public AppointmentNotFoundException(int appointmentId)
        : base($"Appointment with ID '{appointmentId}' was not found.")
    {
        AppointmentId = appointmentId;
    }

    public int AppointmentId { get; }
}

public sealed class AppointmentStateTransitionException : DomainException
{
    public AppointmentStateTransitionException(int appointmentId, AppointmentStatus currentStatus, AppointmentStatus targetStatus)
        : base($"Cannot transition appointment '{appointmentId}' from '{currentStatus}' to '{targetStatus}'.")
    {
        AppointmentId = appointmentId;
        CurrentStatus = currentStatus;
        TargetStatus = targetStatus;
    }

    public int AppointmentId { get; }
    public AppointmentStatus CurrentStatus { get; }
    public AppointmentStatus TargetStatus { get; }
}

public sealed class SlotAlreadyBookedException : DomainException
{
    public SlotAlreadyBookedException(int slotId)
        : base($"Appointment slot '{slotId}' is already booked.")
    {
        SlotId = slotId;
    }

    public int SlotId { get; }
}
