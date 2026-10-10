namespace MyApp.UnitTests.Identity;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Domain.Entities;
using MyApp.Identity.Infrastructure.Authentication;
using MyApp.Identity.Infrastructure.Persistence.DbContext;
using MyApp.Shared.Application.Abstractions;
using MyApp.Shared.Domain;
using MyApp.Shared.Infrastructure.Observability.Metrics;
using Xunit;

public class LoginHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly AuthenticationMetrics _metrics;
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userRoleRepositoryMock = new Mock<IUserRoleRepository>();
        _metrics = new AuthenticationMetrics();

        var secretKeyBytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(32);
        var secretKey = Convert.ToBase64String(secretKeyBytes);

        var jwtOptions = new JwtOptions
        {
            Issuer = "test-issuer",
            Audience = "test-audience",
            SecretKey = secretKey,
            AccessTokenExpirationMinutes = 30,
            RefreshTokenExpirationDays = 7
        };

        _passwordHasher = new PasswordHasher();
        var accessTokenGenerator = new AccessTokenGenerator(Options.Create(jwtOptions));
        _tokenService = new TokenService(
            accessTokenGenerator,
            Mock.Of<IRefreshTokenRepository>(),
            Mock.Of<IdentityDbContext>(),
            Options.Create(jwtOptions));

        _handler = new LoginHandler(
            _userRepositoryMock.Object,
            _userRoleRepositoryMock.Object,
            _passwordHasher,
            _tokenService,
            _metrics);
    }

    [Fact]
    public async Task Handle_NonExistentUser_ReturnsFailure()
    {
        var command = new LoginCommand("nonexistent@example.com", "password123");
        _userRepositoryMock.Setup(x => x.GetByEmailAsync("nonexistent@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Auth.InvalidCredentials", result.Error.Code);
    }

    [Fact]
    public async Task Handle_InactiveUser_ReturnsFailure()
    {
        var user = CreateTestUser("inactive@example.com", isActive: false);
        var command = new LoginCommand("inactive@example.com", "password123");
        _userRepositoryMock.Setup(x => x.GetByEmailAsync("inactive@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Auth.InvalidCredentials", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WrongPassword_ReturnsFailure()
    {
        var password = "CorrectPassword123!";
        var wrongPassword = "WrongPassword123!";
        var user = CreateTestUserWithPassword("test@example.com", password);
        var command = new LoginCommand("test@example.com", wrongPassword);
        _userRepositoryMock.Setup(x => x.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Auth.InvalidCredentials", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsTokenPair()
    {
        var password = "CorrectPassword123!";
        var user = CreateTestUserWithPassword("test@example.com", password);
        var userRoles = CreateTestUserRoles(user.Id, ["Doctor", "Admin"]);
        var command = new LoginCommand("test@example.com", password);

        _userRepositoryMock.Setup(x => x.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userRoleRepositoryMock.Setup(x => x.GetByUserIdIncludingRoleAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userRoles);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value.AccessToken.Token);
        Assert.Equal(1800, result.Value.AccessToken.ExpiresIn);
        Assert.NotEmpty(result.Value.RefreshToken);
    }

    [Fact]
    public async Task Handle_UserWithNoRoles_ReturnsTokenPairWithEmptyRoles()
    {
        var password = "CorrectPassword123!";
        var user = CreateTestUserWithPassword("test@example.com", password);
        var command = new LoginCommand("test@example.com", password);

        _userRepositoryMock.Setup(x => x.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userRoleRepositoryMock.Setup(x => x.GetByUserIdIncludingRoleAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value.AccessToken.Token);
    }

    private static User CreateTestUser(string email, bool isActive = true)
    {
        var passwordHash = new PasswordHasher().Hash("SomePassword");
        var result = User.Create(email, passwordHash, "John", "Doe", null, null);
        var user = result.Value;

        if (!isActive)
        {
            user.Deactivate();
        }

        return user;
    }

    private static User CreateTestUserWithPassword(string email, string password)
    {
        var passwordHash = new PasswordHasher().Hash(password);
        var result = User.Create(email, passwordHash, "John", "Doe", null, null);
        return result.Value;
    }

    private static IReadOnlyList<UserRole> CreateTestUserRoles(Guid userId, IEnumerable<string> roleNames)
    {
        var roles = new List<UserRole>();
        foreach (var roleName in roleNames)
        {
            var roleResult = Role.Create(roleName, null);
            var role = roleResult.Value;
            var userRoleResult = UserRole.CreateGlobal(userId, role.Id);
            var userRole = userRoleResult.Value;
            userRole.GetType().GetProperty("Role")?.SetValue(userRole, role);
            roles.Add(userRole);
        }
        return roles;
    }
}