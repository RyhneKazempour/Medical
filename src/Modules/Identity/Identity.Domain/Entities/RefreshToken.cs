namespace MyApp.Identity.Domain.Entities;

using MyApp.Shared.Domain;

public sealed class RefreshToken : AuditableEntity
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }
    public string? CreatedByIpAddress { get; private set; }
    public string? RevokedByIpAddress { get; private set; }

    public User? User { get; private set; }
    public RefreshToken? ReplacedByToken { get; private set; }

    private RefreshToken() { }

    private RefreshToken(Guid id, Guid userId, string tokenHash, DateTimeOffset expiresAt, string? createdByIpAddress)
        : base(id)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        CreatedByIpAddress = createdByIpAddress;
        IsDeleted = false;
    }

    public static Result<RefreshToken> Create(Guid userId, string tokenHash, DateTimeOffset expiresAt, string? createdByIpAddress)
    {
        if (userId == Guid.Empty)
            return Result<RefreshToken>.Failure(new Error("RefreshToken.UserIdRequired", "User ID is required."));

        if (string.IsNullOrWhiteSpace(tokenHash))
            return Result<RefreshToken>.Failure(new Error("RefreshToken.TokenHashRequired", "Token hash is required."));

        if (expiresAt <= DateTimeOffset.UtcNow)
            return Result<RefreshToken>.Failure(new Error("RefreshToken.ExpiresAtInvalid", "Expiration must be in the future."));

        var refreshToken = new RefreshToken(Guid.NewGuid(), userId, tokenHash, expiresAt, createdByIpAddress?.Trim());
        return Result<RefreshToken>.Success(refreshToken);
    }

    public void Revoke(string? revokedByIpAddress, Guid? replacedByTokenId = null)
    {
        RevokedAt = DateTimeOffset.UtcNow;
        RevokedByIpAddress = revokedByIpAddress?.Trim();
        ReplacedByTokenId = replacedByTokenId;
    }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTimeOffset.UtcNow && !IsDeleted;
}