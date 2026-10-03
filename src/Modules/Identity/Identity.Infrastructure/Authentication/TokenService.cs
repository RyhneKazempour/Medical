namespace MyApp.Identity.Infrastructure.Authentication;

using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Identity.Domain.Entities;
using MyApp.Identity.Infrastructure.Persistence.DbContext;
using MyApp.Shared.Application.Abstractions;
using MyApp.Shared.Domain;

internal sealed class TokenService : ITokenService
{
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IdentityDbContext _context;
    private readonly JwtOptions _jwtOptions;

    public TokenService(
        IAccessTokenGenerator accessTokenGenerator,
        IRefreshTokenRepository refreshTokenRepository,
        IdentityDbContext context,
        IOptions<JwtOptions> jwtOptions)
    {
        _accessTokenGenerator = accessTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _context = context;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<TokenPair> GenerateTokenPairAsync(AccessTokenPrincipal principal, string? clientIpAddress = null)
    {
        var accessToken = _accessTokenGenerator.GenerateToken(principal);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenHash = HashRefreshToken(refreshToken);
        var expiresAt = DateTimeOffset.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);

        var refreshTokenEntity = RefreshToken.Create(principal.UserId, refreshTokenHash, expiresAt, clientIpAddress);
        if (refreshTokenEntity.IsFailure)
        {
            throw new InvalidOperationException($"Failed to create refresh token: {refreshTokenEntity.Error.Description}");
        }

        await _refreshTokenRepository.AddAsync(refreshTokenEntity.Value);
        await _context.SaveChangesAsync();

        return new TokenPair(accessToken, refreshToken, (int)_jwtOptions.RefreshTokenExpirationDays * 24 * 60 * 60);
    }

    public async Task<Result<TokenPair>> RefreshTokenAsync(string refreshToken, string? clientIpAddress, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result<TokenPair>.Failure(new Error("Token.RefreshTokenRequired", "Refresh token is required."));
        }

        var refreshTokenHash = HashRefreshToken(refreshToken);
        var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(refreshTokenHash, cancellationToken);

        if (storedToken is null)
        {
            return Result<TokenPair>.Failure(new Error("Token.InvalidRefreshToken", "Invalid refresh token."));
        }

        if (!storedToken.IsActive)
        {
            return Result<TokenPair>.Failure(new Error("Token.RefreshTokenExpired", "Refresh token has expired or been revoked."));
        }

        var user = storedToken.User;
        if (user is null || !user.IsActive)
        {
            return Result<TokenPair>.Failure(new Error("Token.UserInactive", "User is inactive."));
        }

        // Revoke the old refresh token (rotation)
        storedToken.Revoke(clientIpAddress);

        // Generate new access token
        var roles = user.UserRoles
            .Where(ur => ur.Role is not null)
            .Select(ur => ur.Role!.Name)
            .Distinct()
            .ToArray();

        var principal = new AccessTokenPrincipal(user.Id, user.Email, roles);
        var newAccessToken = _accessTokenGenerator.GenerateToken(principal);

        // Generate new refresh token (active, not revoked)
        var newRefreshToken = GenerateRefreshToken();
        var newRefreshTokenHash = HashRefreshToken(newRefreshToken);
        var newExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);

        var newRefreshTokenEntity = RefreshToken.Create(user.Id, newRefreshTokenHash, newExpiresAt, clientIpAddress);
        if (newRefreshTokenEntity.IsFailure)
        {
            return Result<TokenPair>.Failure(newRefreshTokenEntity.Error);
        }

        // Persist both changes atomically
        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity.Value, cancellationToken);
        await _refreshTokenRepository.UpdateAsync(storedToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<TokenPair>.Success(new TokenPair(
            newAccessToken,
            newRefreshToken,
            (int)_jwtOptions.RefreshTokenExpirationDays * 24 * 60 * 60));
    }

    private static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string HashRefreshToken(string refreshToken)
    {
        using var sha256 = SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(refreshToken);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
