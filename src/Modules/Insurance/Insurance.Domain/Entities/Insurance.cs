namespace MyApp.Insurance.Domain.Entities;

using MyApp.Shared.Domain;

public sealed class Insurance : AuditableActivatableEntity
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public string? ContactPhone { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? Address { get; private set; }

    private PatientInsurance[] _patientInsurances = [];
    public IReadOnlyCollection<PatientInsurance> PatientInsurances => _patientInsurances;

    private Insurance() { }

    private Insurance(Guid id, string name, string code, string? contactPhone, string? contactEmail, string? address)
        : base(id)
    {
        Name = name;
        Code = code;
        ContactPhone = contactPhone;
        ContactEmail = contactEmail;
        Address = address;
        IsActive = true;
        IsDeleted = false;
    }

    public static Result<Insurance> Create(string name, string code, string? contactPhone, string? contactEmail, string? address)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Insurance>.Failure(new Error("Insurance.NameRequired", "Insurance name is required."));

        if (string.IsNullOrWhiteSpace(code))
            return Result<Insurance>.Failure(new Error("Insurance.CodeRequired", "Insurance code is required."));

        var insurance = new Insurance(Guid.NewGuid(), name.Trim(), code.Trim().ToUpperInvariant(), contactPhone?.Trim(), contactEmail?.Trim(), address?.Trim());
        return Result<Insurance>.Success(insurance);
    }

    public Result Update(string? name, string? code, string? contactPhone, string? contactEmail, string? address, bool? isActive)
    {
        if (name is not null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure(new Error("Insurance.NameRequired", "Insurance name is required."));
            Name = name.Trim();
        }

        if (code is not null)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Result.Failure(new Error("Insurance.CodeRequired", "Insurance code is required."));
            Code = code.Trim().ToUpperInvariant();
        }

        if (contactPhone is not null) ContactPhone = contactPhone.Trim();
        if (contactEmail is not null) ContactEmail = contactEmail.Trim();
        if (address is not null) Address = address.Trim();
        if (isActive.HasValue) IsActive = isActive.Value;

        return Result.Success();
    }

    public void AddPatientInsurance(PatientInsurance patientInsurance) => _patientInsurances = [.. _patientInsurances, patientInsurance];
}
