namespace MyApp.Medical.Domain.Entities;

using MyApp.Shared.Domain;

public sealed class Patient : AuditableEntity
{
    public Guid UserId { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public string? Gender { get; private set; }
    public string? BloodType { get; private set; }
    public string? EmergencyContactName { get; private set; }
    public string? EmergencyContactPhone { get; private set; }
    public string? Address { get; private set; }

    private Patient() { }

    private Patient(Guid id, Guid userId, DateOnly? dateOfBirth, string? gender, string? bloodType,
        string? emergencyContactName, string? emergencyContactPhone, string? address)
        : base(id)
    {
        UserId = userId;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        BloodType = bloodType;
        EmergencyContactName = emergencyContactName;
        EmergencyContactPhone = emergencyContactPhone;
        Address = address;
        IsDeleted = false;
    }

    public static Result<Patient> Create(Guid userId, DateOnly? dateOfBirth, string? gender, string? bloodType,
        string? emergencyContactName, string? emergencyContactPhone, string? address)
    {
        if (userId == Guid.Empty)
            return Result<Patient>.Failure(new Error("Patient.UserIdRequired", "User ID is required."));

        var patient = new Patient(Guid.NewGuid(), userId, dateOfBirth, gender?.Trim(), bloodType?.Trim(),
            emergencyContactName?.Trim(), emergencyContactPhone?.Trim(), address?.Trim());
        return Result<Patient>.Success(patient);
    }

    public Result Update(DateOnly? dateOfBirth, string? gender, string? bloodType,
        string? emergencyContactName, string? emergencyContactPhone, string? address)
    {
        DateOfBirth = dateOfBirth;
        Gender = gender?.Trim();
        BloodType = bloodType?.Trim();
        EmergencyContactName = emergencyContactName?.Trim();
        EmergencyContactPhone = emergencyContactPhone?.Trim();
        Address = address?.Trim();

        return Result.Success();
    }
}
