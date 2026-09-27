namespace MyApp.Medical.Domain.Entities;

using MyApp.Shared.Domain;

public sealed class Doctor : AuditableActivatableEntity
{
    public Guid UserId { get; private set; }
    public string LicenseNumber { get; private set; } = null!;
    public string? Bio { get; private set; }

    private DoctorSpecialization[] _doctorSpecializations = [];
    public IReadOnlyCollection<DoctorSpecialization> DoctorSpecializations => _doctorSpecializations;

    private DoctorHospital[] _doctorHospitals = [];
    public IReadOnlyCollection<DoctorHospital> DoctorHospitals => _doctorHospitals;

    private Doctor() { }

    private Doctor(Guid id, Guid userId, string licenseNumber, string? bio)
        : base(id)
    {
        UserId = userId;
        LicenseNumber = licenseNumber;
        Bio = bio;
        IsActive = true;
        IsDeleted = false;
    }

    public static Result<Doctor> Create(Guid userId, string licenseNumber, string? bio)
    {
        if (userId == Guid.Empty)
            return Result<Doctor>.Failure(new Error("Doctor.UserIdRequired", "User ID is required."));

        if (string.IsNullOrWhiteSpace(licenseNumber))
            return Result<Doctor>.Failure(new Error("Doctor.LicenseNumberRequired", "License number is required."));

        var doctor = new Doctor(Guid.NewGuid(), userId, licenseNumber.Trim(), bio?.Trim());
        return Result<Doctor>.Success(doctor);
    }

    public Result Update(string? licenseNumber, string? bio, bool? isActive)
    {
        if (licenseNumber is not null)
        {
            if (string.IsNullOrWhiteSpace(licenseNumber))
                return Result.Failure(new Error("Doctor.LicenseNumberRequired", "License number is required."));
            LicenseNumber = licenseNumber.Trim();
        }

        if (bio is not null) Bio = bio.Trim();
        if (isActive.HasValue) IsActive = isActive.Value;

        return Result.Success();
    }

    public Result AddSpecialization(DoctorSpecialization doctorSpecialization)
    {
        if (_doctorSpecializations.Any(ds => ds.SpecializationId == doctorSpecialization.SpecializationId))
            return Result.Failure(new Error("Doctor.SpecializationExists", "Doctor already has this specialization."));

        _doctorSpecializations = [.. _doctorSpecializations, doctorSpecialization];
        return Result.Success();
    }

    public void RemoveSpecialization(Guid specializationId)
    {
        _doctorSpecializations = _doctorSpecializations.Where(ds => ds.SpecializationId != specializationId).ToArray();
    }

    public void AddDoctorHospital(DoctorHospital doctorHospital) => _doctorHospitals = [.. _doctorHospitals, doctorHospital];
}
