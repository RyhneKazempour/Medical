namespace MyApp.Medical.Domain.Entities;

using MyApp.Shared.Domain;

public sealed class Specialization : AuditableActivatableEntity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    private DoctorSpecialization[] _doctorSpecializations = [];
    public IReadOnlyCollection<DoctorSpecialization> DoctorSpecializations => _doctorSpecializations;

    private Specialization() { }

    private Specialization(Guid id, string name, string? description)
        : base(id)
    {
        Name = name;
        Description = description;
        IsActive = true;
        IsDeleted = false;
    }

    public static Result<Specialization> Create(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Specialization>.Failure(new Error("Specialization.NameRequired", "Specialization name is required."));

        var specialization = new Specialization(Guid.NewGuid(), name.Trim(), description?.Trim());
        return Result<Specialization>.Success(specialization);
    }

    public Result Update(string? name, string? description, bool? isActive)
    {
        if (name is not null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure(new Error("Specialization.NameRequired", "Specialization name is required."));
            Name = name.Trim();
        }

        if (description is not null) Description = description.Trim();
        if (isActive.HasValue) IsActive = isActive.Value;

        return Result.Success();
    }

    public void AddDoctorSpecialization(DoctorSpecialization doctorSpecialization) => _doctorSpecializations = [.. _doctorSpecializations, doctorSpecialization];
}
