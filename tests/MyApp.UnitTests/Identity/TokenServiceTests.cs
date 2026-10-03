namespace MyApp.UnitTests.Identity;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Identity.Domain.Entities;
using MyApp.Identity.Infrastructure.Authentication;
using MyApp.Identity.Infrastructure.Persistence.DbContext;
using MyApp.Shared.Application.Abstractions;
using MyApp.Shared.Domain;
using Xunit;

public class TokenServiceTests
{
    private readonly JwtOptions _jwtOptions;
    private readonly Mock<IAccessTokenGenerator> _accessTokenGeneratorMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IdentityDbContext> _contextMock;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        var secretKeyBytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(32);
        var secretKey = Convert.ToBase64String(secretKeyBytes);

        _jwtOptions = new JwtOptions
        {
            Issuer = "test-issuer",
            Audience = "test-audience",
            SecretKey = secretKey,
            AccessTokenExpirationMinutes = 30,
            RefreshTokenExpirationDays = 7
        };

        _accessTokenGeneratorMock = new Mock<IAccessTokenGenerator>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _contextMock = new Mock<IdentityDbContext>();

        var accessToken = new AccessToken("test-access-token", 1800);
        _accessTokenGeneratorMock.Setup(x => x.GenerateToken(It.IsAny<AccessTokenPrincipal>()))
            .Returns(accessToken);

        _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _tokenService = new TokenService(
            _accessTokenGeneratorMock.Object,
            _refreshTokenRepositoryMock.Object,
            _contextMock.Object,
            Options.Create(_jwtOptions));
    }

    [Fact]
    public async Task GenerateTokenPairAsync_ValidPrincipal_ReturnsTokenPair()
    {
        var principal = new AccessTokenPrincipal(Guid.NewGuid(), "test@example.com", ["Doctor"]);

        _refreshTokenRepositoryMock.Setup(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var tokenPair = await _tokenService.GenerateTokenPairAsync(principal, "192.168.1.1");

        Assert.NotNull(tokenPair);
        Assert.Equal("test-access-token", tokenPair.AccessToken.Token);
        Assert.Equal(1800, tokenPair.AccessToken.ExpiresIn);
        Assert.NotEmpty(tokenPair.RefreshToken);
        Assert.Equal(7 * 24 * 60 * 60, tokenPair.RefreshTokenExpiresIn);

        _accessTokenGeneratorMock.Verify(x => x.GenerateToken(It.Is<AccessTokenPrincipal>(p =>
            p.UserId == principal.UserId && p.Email == principal.Email && p.Roles.SequenceEqual(principal.Roles))), Times.Once);
        _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokenPair()
    {
        var userId = Guid.NewGuid();
        var oldRefreshToken = "old-refresh-token";
        var oldRefreshTokenHash = HashRefreshToken(oldRefreshToken);

        var user = CreateTestUser(userId);
        var oldTokenEntity = CreateRefreshTokenEntity(userId, oldRefreshTokenHash, user);
        // Don't revoke - token should be active

        _refreshTokenRepositoryMock.Setup(x => x.GetByTokenHashAsync(oldRefreshTokenHash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldTokenEntity);

        _refreshTokenRepositoryMock.Setup(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _refreshTokenRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _tokenService.RefreshTokenAsync(oldRefreshToken, "192.168.1.1", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("test-access-token", result.Value.AccessToken.Token);
        Assert.NotEmpty(result.Value.RefreshToken);
        Assert.NotEqual(oldRefreshToken, result.Value.RefreshToken);

        _refreshTokenRepositoryMock.Verify(x => x.GetByTokenHashAsync(oldRefreshTokenHash, It.IsAny<CancellationToken>()), Times.Once);
        _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        _refreshTokenRepositoryMock.Verify(x => x.UpdateAsync(It.Is<RefreshToken>(t => t.Id == oldTokenEntity.Id), It.IsAny<CancellationToken>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_EmptyToken_ReturnsFailure()
    {
        var result = await _tokenService.RefreshTokenAsync("", "192.168.1.1", CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Token.RefreshTokenRequired", result.Error.Code);
    }

    [Fact]
    public async Task RefreshTokenAsync_NonExistentToken_ReturnsFailure()
    {
        var refreshToken = "non-existent-token";
        var refreshTokenHash = HashRefreshToken(refreshToken);

        _refreshTokenRepositoryMock.Setup(x => x.GetByTokenHashAsync(refreshTokenHash, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        var result = await _tokenService.RefreshTokenAsync(refreshToken, "192.168.1.1", CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Token.InvalidRefreshToken", result.Error.Code);
    }

    [Fact]
    public async Task RefreshTokenAsync_ExpiredToken_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var refreshToken = "expired-refresh-token";
        var refreshTokenHash = HashRefreshToken(refreshToken);

        var user = CreateTestUser(userId);
        var oldTokenEntity = CreateRefreshTokenEntity(userId, refreshTokenHash, user);
        // Make it expired by setting RevokedAt
        oldTokenEntity.Revoke(null);

        _refreshTokenRepositoryMock.Setup(x => x.GetByTokenHashAsync(refreshTokenHash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldTokenEntity);

        var result = await _tokenService.RefreshTokenAsync(refreshToken, "192.168.1.1", CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Token.RefreshTokenExpired", result.Error.Code);
    }

    [Fact]
    public async Task RefreshTokenAsync_InactiveUser_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var refreshToken = "valid-refresh-token";
        var refreshTokenHash = HashRefreshToken(refreshToken);

        var user = CreateTestUser(userId);
        user.Deactivate(); // Make user inactive
        var oldTokenEntity = CreateRefreshTokenEntity(userId, refreshTokenHash, user);

        _refreshTokenRepositoryMock.Setup(x => x.GetByTokenHashAsync(refreshTokenHash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldTokenEntity);

        var result = await _tokenService.RefreshTokenAsync(refreshToken, "192.168.1.1", CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Token.UserInactive", result.Error.Code);
    }

    [Fact]
    public async Task RefreshTokenAsync_RevokedToken_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var refreshToken = "revoked-refresh-token";
        var refreshTokenHash = HashRefreshToken(refreshToken);

        var user = CreateTestUser(userId);
        var oldTokenEntity = CreateRefreshTokenEntity(userId, refreshTokenHash, user);
        oldTokenEntity.Revoke("192.168.1.1"); // Revoke the token

        _refreshTokenRepositoryMock.Setup(x => x.GetByTokenHashAsync(refreshTokenHash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldTokenEntity);

        var result = await _tokenService.RefreshTokenAsync(refreshToken, "192.168.1.1", CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Token.RefreshTokenExpired", result.Error.Code);
    }

    private static User CreateTestUser(Guid userId)
    {
        var passwordHash = new PasswordHasher().Hash("password123");
        var result = User.Create("test@example.com", passwordHash, "John", "Doe", null, null);
        var user = result.Value;
        user.GetType().GetProperty("Id")?.SetValue(user, userId);
        return user;
    }

    private static RefreshToken CreateRefreshTokenEntity(Guid userId, string tokenHash, User user)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddDays(7);
        var result = RefreshToken.Create(userId, tokenHash, expiresAt, "192.168.1.1");
        var token = result.Value;
        token.GetType().GetProperty("User")?.SetValue(token, user);
        return token;
    }

    private static string HashRefreshToken(string refreshToken)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(refreshToken);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
