namespace MyApp.Identity.Domain.Entities;

using MyApp.Shared.Domain;

public sealed class User : AuditableActivatableEntity
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string? Phone { get; private set; }
    public string? Mobile { get; private set; }

    private UserRole[] _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

    private User() { }

    private User(Guid id, string email, string passwordHash, string firstName, string lastName, string? phone, string? mobile)
        : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Mobile = NormalizeMobile(mobile);
        IsActive = true;
        IsDeleted = false;
    }

    public static Result<User> Create(string email, string passwordHash, string firstName, string lastName, string? phone, string? mobile)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<User>.Failure(new Error("User.EmailRequired", "Email is required."));

        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result<User>.Failure(new Error("User.PasswordRequired", "Password hash is required."));

        if (string.IsNullOrWhiteSpace(firstName))
            return Result<User>.Failure(new Error("User.FirstNameRequired", "First name is required."));

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<User>.Failure(new Error("User.LastNameRequired", "Last name is required."));

        var user = new User(Guid.NewGuid(), email.Trim().ToLowerInvariant(), passwordHash, firstName.Trim(), lastName.Trim(), phone?.Trim(), mobile?.Trim());
        return Result<User>.Success(user);
    }

    public Result UpdateProfile(string firstName, string lastName, string? phone, string? mobile)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure(new Error("User.FirstNameRequired", "First name is required."));

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure(new Error("User.LastNameRequired", "Last name is required."));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Phone = phone?.Trim();
        Mobile = NormalizeMobile(mobile);

        return Result.Success();
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public static string? NormalizeMobile(string? mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile))
            return null;

        // Remove all non-digit characters except leading +
        var normalized = new string(mobile.Where(c => char.IsDigit(c) || c == '+').ToArray());

        // Ensure it starts with + if it has country code
        if (!normalized.StartsWith('+') && normalized.Length > 10)
            normalized = "+" + normalized;

        return normalized;
    }
}
