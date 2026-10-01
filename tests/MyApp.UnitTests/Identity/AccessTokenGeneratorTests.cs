namespace MyApp.UnitTests.Identity;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyApp.Identity.Application.Authentication;
using MyApp.Identity.Infrastructure.Authentication;
using Xunit;

public class AccessTokenGeneratorTests
{
    private readonly JwtOptions _testOptions;
    private readonly AccessTokenGenerator _generator;

    public AccessTokenGeneratorTests()
    {
        var secretKeyBytes = RandomNumberGenerator.GetBytes(32);
        var secretKey = Convert.ToBase64String(secretKeyBytes);

        _testOptions = new JwtOptions
        {
            Issuer = "test-issuer",
            Audience = "test-audience",
            SecretKey = secretKey,
            AccessTokenExpirationMinutes = 30
        };

        _generator = new AccessTokenGenerator(Options.Create(_testOptions));
    }

    private TokenValidationParameters CreateValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _testOptions.Issuer,
            ValidAudience = _testOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_testOptions.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    }

    [Fact]
    public void GenerateToken_ValidPrincipal_ReturnsTokenWithCorrectClaims()
    {
        var userId = Guid.NewGuid();
        var principal = new AccessTokenPrincipal(userId, "test@example.com", ["SuperAdmin", "Admin"]);

        var accessToken = _generator.GenerateToken(principal);

        Assert.NotNull(accessToken);
        Assert.NotEmpty(accessToken.Token);
        Assert.Equal(1800, accessToken.ExpiresIn);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(accessToken.Token);

        Assert.Equal(_testOptions.Issuer, jwtToken.Issuer);
        Assert.Equal(_testOptions.Audience, jwtToken.Audiences.First());

        var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        Assert.NotNull(subClaim);
        Assert.Equal(userId.ToString(), subClaim.Value);

        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        Assert.NotNull(emailClaim);
        Assert.Equal("test@example.com", emailClaim.Value);

        var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        Assert.Equal(2, roleClaims.Count);
        Assert.Contains(roleClaims, c => c.Value == "SuperAdmin");
        Assert.Contains(roleClaims, c => c.Value == "Admin");

        var jtiClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
        Assert.NotNull(jtiClaim);

        var iatClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat);
        Assert.NotNull(iatClaim);
    }

    [Fact]
    public void GenerateToken_GeneratedToken_ValidatesSuccessfully()
    {
        var principal = new AccessTokenPrincipal(Guid.NewGuid(), "test@example.com", ["Doctor"]);
        var accessToken = _generator.GenerateToken(principal);

        var handler = new JwtSecurityTokenHandler();
        var validationParameters = CreateValidationParameters();

        var principalResult = handler.ValidateToken(accessToken.Token, validationParameters, out var validatedToken);

        Assert.NotNull(principalResult);
        Assert.Equal("Doctor", principalResult.FindFirst(ClaimTypes.Role)?.Value);
        Assert.Equal(_testOptions.Issuer, ((JwtSecurityToken)validatedToken).Issuer);
    }

    [Fact]
    public void GenerateToken_ExpiredToken_IsRejected()
    {
        // Generate a valid token first, then test validation with an expired token
        // We can't generate with negative expiry as it throws in the generator
        // Instead, create a token with a past expiration by manually constructing one
        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Convert.FromBase64String(_testOptions.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var expiredToken = new JwtSecurityToken(
            issuer: _testOptions.Issuer,
            audience: _testOptions.Audience,
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.AddMinutes(-10).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Email, "test@example.com"),
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim(ClaimTypes.Role, "Doctor"),
            },
            notBefore: DateTimeOffset.UtcNow.AddMinutes(-10).UtcDateTime,
            expires: DateTimeOffset.UtcNow.AddMinutes(-5).UtcDateTime, // Expired 5 minutes ago
            signingCredentials: credentials);

        var tokenString = handler.WriteToken(expiredToken);

        var validationParameters = CreateValidationParameters();

        Assert.Throws<SecurityTokenExpiredException>(() =>
            handler.ValidateToken(tokenString, validationParameters, out _));
    }

    [Fact]
    public void GenerateToken_InvalidSignature_IsRejected()
    {
        var principal = new AccessTokenPrincipal(Guid.NewGuid(), "test@example.com", ["Doctor"]);
        var accessToken = _generator.GenerateToken(principal);

        // Tamper with the signature by changing the last segment
        var parts = accessToken.Token.Split('.');
        var tamperedToken = parts[0] + "." + parts[1] + "." + parts[2] + "x";

        var handler = new JwtSecurityTokenHandler();
        var validationParameters = CreateValidationParameters();

        Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() =>
            handler.ValidateToken(tamperedToken, validationParameters, out _));
    }

    [Fact]
    public void GenerateToken_InvalidIssuer_IsRejected()
    {
        var principal = new AccessTokenPrincipal(Guid.NewGuid(), "test@example.com", ["Doctor"]);
        var accessToken = _generator.GenerateToken(principal);

        var validationParameters = CreateValidationParameters();
        validationParameters.ValidIssuer = "wrong-issuer";

        var handler = new JwtSecurityTokenHandler();

        Assert.Throws<SecurityTokenInvalidIssuerException>(() =>
            handler.ValidateToken(accessToken.Token, validationParameters, out _));
    }

    [Fact]
    public void GenerateToken_InvalidAudience_IsRejected()
    {
        var principal = new AccessTokenPrincipal(Guid.NewGuid(), "test@example.com", ["Doctor"]);
        var accessToken = _generator.GenerateToken(principal);

        var validationParameters = CreateValidationParameters();
        validationParameters.ValidAudience = "wrong-audience";

        var handler = new JwtSecurityTokenHandler();

        Assert.Throws<SecurityTokenInvalidAudienceException>(() =>
            handler.ValidateToken(accessToken.Token, validationParameters, out _));
    }

    [Fact]
    public void GenerateToken_MalformedToken_IsRejected()
    {
        var handler = new JwtSecurityTokenHandler();
        var validationParameters = CreateValidationParameters();

        Assert.Throws<SecurityTokenMalformedException>(() =>
            handler.ValidateToken("not.a.valid.token", validationParameters, out _));
    }
}