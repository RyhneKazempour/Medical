namespace MyApp.Medical.Domain.Entities;

using MyApp.Shared.Domain;
using System.ComponentModel.DataAnnotations.Schema;

public sealed class DoctorHospital : AuditableActivatableEntity
{
    public Guid DoctorSpecializationId { get; private set; }
    public Guid ClinicId { get; private set; }
    public string? RoomNumber { get; private set; }
    public DateOnly StartContractDate { get; private set; }
    public DateOnly? EndContractDate { get; private set; }

    [ForeignKey(nameof(DoctorSpecializationId))]
    public DoctorSpecialization? DoctorSpecialization { get; private set; }

    [ForeignKey(nameof(ClinicId))]
    public Clinic? Clinic { get; private set; }

    private DoctorHospital() { }

    private DoctorHospital(Guid id, Guid doctorSpecializationId, Guid clinicId, string? roomNumber,
        DateOnly startContractDate, DateOnly? endContractDate)
        : base(id)
    {
        DoctorSpecializationId = doctorSpecializationId;
        ClinicId = clinicId;
        RoomNumber = roomNumber;
        StartContractDate = startContractDate;
        EndContractDate = endContractDate;
        IsActive = true;
        IsDeleted = false;
    }

    public static Result<DoctorHospital> Create(Guid doctorSpecializationId, Guid clinicId, string? roomNumber,
        DateOnly startContractDate, DateOnly? endContractDate)
    {
        if (doctorSpecializationId == Guid.Empty)
            return Result<DoctorHospital>.Failure(new Error("DoctorHospital.DoctorSpecializationIdRequired", "Doctor specialization ID is required."));

        if (clinicId == Guid.Empty)
            return Result<DoctorHospital>.Failure(new Error("DoctorHospital.ClinicIdRequired", "Clinic ID is required."));

        if (endContractDate.HasValue && endContractDate.Value < startContractDate)
            return Result<DoctorHospital>.Failure(new Error("DoctorHospital.InvalidContractDates", "End contract date cannot be before start contract date."));

        var doctorHospital = new DoctorHospital(Guid.NewGuid(), doctorSpecializationId, clinicId, roomNumber?.Trim(), startContractDate, endContractDate);
        return Result<DoctorHospital>.Success(doctorHospital);
    }

    public Result Update(string? roomNumber, DateOnly? startContractDate, DateOnly? endContractDate, bool? isActive)
    {
        if (roomNumber is not null) RoomNumber = roomNumber.Trim();
        if (startContractDate.HasValue) StartContractDate = startContractDate.Value;
        if (endContractDate.HasValue) EndContractDate = endContractDate.Value;

        if (EndContractDate.HasValue && EndContractDate.Value < StartContractDate)
            return Result.Failure(new Error("DoctorHospital.InvalidContractDates", "End contract date cannot be before start contract date."));

        if (isActive.HasValue) IsActive = isActive.Value;

        return Result.Success();
    }

    public bool IsContractValid(DateOnly date) =>
        date >= StartContractDate && (!EndContractDate.HasValue || date <= EndContractDate.Value);
}
