namespace MyApp.Medical.Domain.Entities;

using MyApp.Shared.Domain;

public sealed class Hospital : AuditableActivatableEntity
{
    public string Name { get; private set; } = null!;
    public string? Address { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }

    private Clinic[] _clinics = [];
    public IReadOnlyCollection<Clinic> Clinics => _clinics;

    private Hospital() { }

    private Hospital(Guid id, string name, string? address, string? phone, string? email)
        : base(id)
    {
        Name = name;
        Address = address;
        Phone = phone;
        Email = email;
        IsActive = true;
        IsDeleted = false;
    }

    public static Result<Hospital> Create(string name, string? address, string? phone, string? email)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Hospital>.Failure(new Error("Hospital.NameRequired", "Hospital name is required."));

        var hospital = new Hospital(Guid.NewGuid(), name.Trim(), address?.Trim(), phone?.Trim(), email?.Trim());
        return Result<Hospital>.Success(hospital);
    }

    public Result Update(string? name, string? address, string? phone, string? email, bool? isActive)
    {
        if (name is not null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure(new Error("Hospital.NameRequired", "Hospital name is required."));
            Name = name.Trim();
        }

        if (address is not null) Address = address.Trim();
        if (phone is not null) Phone = phone.Trim();
        if (email is not null) Email = email.Trim();
        if (isActive.HasValue) IsActive = isActive.Value;

        return Result.Success();
    }

    public void AddClinic(Clinic clinic) => _clinics = [.. _clinics, clinic];
}
