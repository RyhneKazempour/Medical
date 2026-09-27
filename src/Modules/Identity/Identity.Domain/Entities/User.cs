namespace MyApp.Identity.Domain.Entities;

using MyApp.Shared.Domain;

public sealed class User : AuditableActivatableEntity
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string? Phone { get; private set; }

    private User() { }

    private User(Guid id, string email, string passwordHash, string firstName, string lastName, string? phone)
        : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        IsActive = true;
        IsDeleted = false;
    }

    public static Result<User> Create(string email, string passwordHash, string firstName, string lastName, string? phone)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<User>.Failure(new Error("User.EmailRequired", "Email is required."));

        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result<User>.Failure(new Error("User.PasswordRequired", "Password hash is required."));

        if (string.IsNullOrWhiteSpace(firstName))
            return Result<User>.Failure(new Error("User.FirstNameRequired", "First name is required."));

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<User>.Failure(new Error("User.LastNameRequired", "Last name is required."));

        var user = new User(Guid.NewGuid(), email.Trim().ToLowerInvariant(), passwordHash, firstName.Trim(), lastName.Trim(), phone?.Trim());
        return Result<User>.Success(user);
    }

    public Result UpdateProfile(string firstName, string lastName, string? phone)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure(new Error("User.FirstNameRequired", "First name is required."));

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure(new Error("User.LastNameRequired", "Last name is required."));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Phone = phone?.Trim();

        return Result.Success();
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void SoftDelete() => IsDeleted = true;
}
