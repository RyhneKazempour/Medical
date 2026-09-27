namespace MyApp.Appointments.Domain.Entities;

using MyApp.Shared.Domain;

public enum AppointmentStatus
{
    Reserved = 1,
    Confirmed = 2,
    Cancelled = 3,
    Completed = 4,
    NoShow = 5
}

public sealed class Appointment : AuditableEntity
{
    public Guid PatientId { get; private set; }
    public Guid SlotId { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTimeOffset ReservedAt { get; private set; }
    public DateTimeOffset? ConfirmedAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public string? CancelReason { get; private set; }

    private Appointment() { }

    private Appointment(Guid id, Guid patientId, Guid slotId)
        : base(id)
    {
        PatientId = patientId;
        SlotId = slotId;
        Status = AppointmentStatus.Reserved;
        ReservedAt = DateTimeOffset.UtcNow;
        IsDeleted = false;
    }

    public static Result<Appointment> Create(Guid patientId, Guid slotId)
    {
        if (patientId == Guid.Empty)
            return Result<Appointment>.Failure(new Error("Appointment.PatientIdRequired", "Patient ID is required."));

        if (slotId == Guid.Empty)
            return Result<Appointment>.Failure(new Error("Appointment.SlotIdRequired", "Slot ID is required."));

        var appointment = new Appointment(Guid.NewGuid(), patientId, slotId);
        return Result<Appointment>.Success(appointment);
    }

    public Result Confirm()
    {
        if (Status != AppointmentStatus.Reserved)
            return Result.Failure(new Error("Appointment.InvalidStateTransition", $"Cannot confirm appointment in '{Status}' state."));

        Status = AppointmentStatus.Confirmed;
        ConfirmedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result Cancel(string reason)
    {
        if (Status == AppointmentStatus.Completed)
            return Result.Failure(new Error("Appointment.InvalidStateTransition", "Cannot cancel a completed appointment."));

        if (Status == AppointmentStatus.Cancelled)
            return Result.Failure(new Error("Appointment.AlreadyCancelled", "Appointment is already cancelled."));

        Status = AppointmentStatus.Cancelled;
        CancelledAt = DateTimeOffset.UtcNow;
        CancelReason = reason.Trim();
        return Result.Success();
    }

    public Result Complete()
    {
        if (Status != AppointmentStatus.Confirmed)
            return Result.Failure(new Error("Appointment.InvalidStateTransition", $"Cannot complete appointment in '{Status}' state."));

        Status = AppointmentStatus.Completed;
        return Result.Success();
    }

    public Result MarkNoShow()
    {
        if (Status != AppointmentStatus.Confirmed)
            return Result.Failure(new Error("Appointment.InvalidStateTransition", $"Cannot mark no-show for appointment in '{Status}' state."));

        Status = AppointmentStatus.NoShow;
        return Result.Success();
    }
}
