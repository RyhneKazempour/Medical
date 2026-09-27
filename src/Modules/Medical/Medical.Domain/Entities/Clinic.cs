namespace MyApp.Medical.Domain.Entities;

using MyApp.Shared.Domain;

public sealed class Clinic : AuditableActivatableEntity
{
    public Guid HospitalId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public Hospital? Hospital { get; private set; }
    private DoctorHospital[] _doctorHospitals = [];
    public IReadOnlyCollection<DoctorHospital> DoctorHospitals => _doctorHospitals;

    private Clinic() { }

    private Clinic(Guid id, Guid hospitalId, string name, string? description)
        : base(id)
    {
        HospitalId = hospitalId;
        Name = name;
        Description = description;
        IsActive = true;
        IsDeleted = false;
    }

    public static Result<Clinic> Create(Guid hospitalId, string name, string? description)
    {
        if (hospitalId == Guid.Empty)
            return Result<Clinic>.Failure(new Error("Clinic.HospitalIdRequired", "Hospital ID is required."));

        if (string.IsNullOrWhiteSpace(name))
            return Result<Clinic>.Failure(new Error("Clinic.NameRequired", "Clinic name is required."));

        var clinic = new Clinic(Guid.NewGuid(), hospitalId, name.Trim(), description?.Trim());
        return Result<Clinic>.Success(clinic);
    }

    public Result Update(string? name, string? description, bool? isActive)
    {
        if (name is not null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure(new Error("Clinic.NameRequired", "Clinic name is required."));
            Name = name.Trim();
        }

        if (description is not null) Description = description.Trim();
        if (isActive.HasValue) IsActive = isActive.Value;

        return Result.Success();
    }

    public void AddDoctorHospital(DoctorHospital doctorHospital) => _doctorHospitals = [.. _doctorHospitals, doctorHospital];
}
