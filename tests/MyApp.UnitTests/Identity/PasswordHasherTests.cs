namespace MyApp.UnitTests.Identity;

using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Infrastructure.Authentication;
using Xunit;

public class PasswordHasherTests
{
    private readonly IPasswordHasher _hasher;

    public PasswordHasherTests()
    {
        _hasher = new PasswordHasher();
    }

    [Fact]
    public void Hash_NonEmptyPassword_ReturnsValidHash()
    {
        var password = "TestPassword123!";
        var hash = _hasher.Hash(password);

        Assert.NotNull(hash);
        Assert.StartsWith("PBKDF2_SHA256$310000$", hash);

        var parts = hash.Split('$');
        Assert.Equal(4, parts.Length);
        Assert.Equal("PBKDF2_SHA256", parts[0]);
        Assert.Equal("310000", parts[1]);
    }

    [Fact]
    public void Hash_SamePasswordDifferentCalls_ProducesDifferentHashes()
    {
        var password = "TestPassword123!";
        var hash1 = _hasher.Hash(password);
        var hash2 = _hasher.Hash(password);

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Verify_CorrectPassword_ReturnsTrue()
    {
        var password = "TestPassword123!";
        var hash = _hasher.Hash(password);

        var result = _hasher.Verify(password, hash);

        Assert.True(result);
    }

    [Fact]
    public void Verify_IncorrectPassword_ReturnsFalse()
    {
        var password = "TestPassword123!";
        var hash = _hasher.Hash(password);

        var result = _hasher.Verify("WrongPassword123!", hash);

        Assert.False(result);
    }

    [Fact]
    public void Verify_EmptyPassword_ReturnsFalse()
    {
        var hash = _hasher.Hash("SomePassword");

        var result = _hasher.Verify("", hash);

        Assert.False(result);
    }

    [Fact]
    public void Verify_EmptyHash_ReturnsFalse()
    {
        var result = _hasher.Verify("password", "");

        Assert.False(result);
    }

    [Fact]
    public void Verify_MalformedHash_ReturnsFalse()
    {
        var result = _hasher.Verify("password", "not-a-valid-hash");

        Assert.False(result);
    }

    [Fact]
    public void Verify_DifferentAlgorithm_ReturnsFalse()
    {
        var hash = "BCRYPT$10$salt$hash";

        var result = _hasher.Verify("password", hash);

        Assert.False(result);
    }

    [Fact]
    public void Verify_WrongIterations_ReturnsFalse()
    {
        var hash = "PBKDF2_SHA256$100000$salt$hash";

        var result = _hasher.Verify("password", hash);

        Assert.False(result);
    }

    [Fact]
    public void Hash_NullPassword_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _hasher.Hash(null!));
    }

    [Fact]
    public void Hash_EmptyPassword_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _hasher.Hash(""));
    }
}