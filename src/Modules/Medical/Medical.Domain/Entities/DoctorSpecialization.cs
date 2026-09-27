namespace MyApp.Medical.Domain.Entities;

using MyApp.Shared.Domain;

public sealed class DoctorSpecialization : AuditableActivatableEntity
{
    public Guid DoctorId { get; private set; }
    public Guid SpecializationId { get; private set; }

    public Doctor? Doctor { get; private set; }
    public Specialization? Specialization { get; private set; }
    private DoctorHospital[] _doctorHospitals = [];
    public IReadOnlyCollection<DoctorHospital> DoctorHospitals => _doctorHospitals;

    private DoctorSpecialization() { }

    private DoctorSpecialization(Guid id, Guid doctorId, Guid specializationId)
        : base(id)
    {
        DoctorId = doctorId;
        SpecializationId = specializationId;
        IsActive = true;
        IsDeleted = false;
    }

    public static Result<DoctorSpecialization> Create(Guid doctorId, Guid specializationId)
    {
        if (doctorId == Guid.Empty)
            return Result<DoctorSpecialization>.Failure(new Error("DoctorSpecialization.DoctorIdRequired", "Doctor ID is required."));

        if (specializationId == Guid.Empty)
            return Result<DoctorSpecialization>.Failure(new Error("DoctorSpecialization.SpecializationIdRequired", "Specialization ID is required."));

        var doctorSpecialization = new DoctorSpecialization(Guid.NewGuid(), doctorId, specializationId);
        return Result<DoctorSpecialization>.Success(doctorSpecialization);
    }

    public void AddDoctorHospital(DoctorHospital doctorHospital) => _doctorHospitals = [.. _doctorHospitals, doctorHospital];
}
