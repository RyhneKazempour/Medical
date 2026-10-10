namespace MyApp.Identity.Domain.Entities;

using MyApp.Shared.Domain;
using MyApp.Identity.Domain.ValueObjects;

public sealed class UserRole : AuditableEntity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public ScopeType ScopeType { get; private set; }
    public Guid ScopeId { get; private set; }
    public long Version { get; private set; }

    public User? User { get; private set; }
    public Role? Role { get; private set; }

    private UserRole() { }

    private UserRole(Guid id, Guid userId, Guid roleId, ScopeType scopeType, Guid scopeId)
        : base(id)
    {
        UserId = userId;
        RoleId = roleId;
        ScopeType = scopeType;
        ScopeId = scopeId;
        IsDeleted = false;
        Version = 1;
    }

    public static Result<UserRole> Create(Guid userId, Guid roleId, ScopeType scopeType, Guid scopeId)
    {
        if (userId == Guid.Empty)
            return Result<UserRole>.Failure(new Error("UserRole.UserIdRequired", "User ID is required."));

        if (roleId == Guid.Empty)
            return Result<UserRole>.Failure(new Error("UserRole.RoleIdRequired", "Role ID is required."));

        if (!IsValidScope(scopeType, scopeId))
            return Result<UserRole>.Failure(new Error("UserRole.InvalidScope", $"Invalid scope combination: {scopeType} with ScopeId {scopeId}"));

        var userRole = new UserRole(Guid.NewGuid(), userId, roleId, scopeType, scopeId);
        return Result<UserRole>.Success(userRole);
    }

    public static Result<UserRole> CreateGlobal(Guid userId, Guid roleId)
    {
        return Create(userId, roleId, ScopeType.Global, Guid.Empty);
    }

    public static Result<UserRole> CreateHospital(Guid userId, Guid roleId, Guid hospitalId)
    {
        if (hospitalId == Guid.Empty)
            return Result<UserRole>.Failure(new Error("UserRole.HospitalIdRequired", "Hospital ID is required for hospital scope."));

        return Create(userId, roleId, ScopeType.Hospital, hospitalId);
    }

    public static Result<UserRole> CreateClinic(Guid userId, Guid roleId, Guid clinicId)
    {
        if (clinicId == Guid.Empty)
            return Result<UserRole>.Failure(new Error("UserRole.ClinicIdRequired", "Clinic ID is required for clinic scope."));

        return Create(userId, roleId, ScopeType.Clinic, clinicId);
    }

    public void IncrementVersion() => Version++;

    public void RestoreUserRole()
    {
        IsDeleted = false;
        IncrementVersion();
    }

    public void SoftDeleteUserRole()
    {
        IsDeleted = true;
        IncrementVersion();
    }

    private static bool IsValidScope(ScopeType scopeType, Guid scopeId)
    {
        return scopeType switch
        {
            ScopeType.Global => scopeId == Guid.Empty,
            ScopeType.Hospital => scopeId != Guid.Empty,
            ScopeType.Clinic => scopeId != Guid.Empty,
            _ => false
        };
    }
}
