namespace MyApp.Identity.Infrastructure.Authentication;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Authentication;

internal sealed class AccessTokenGenerator : IAccessTokenGenerator
{
    private readonly JwtOptions _options;

    public AccessTokenGenerator(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AccessToken GenerateToken(AccessTokenPrincipal principal)
    {
        var now = DateTimeOffset.UtcNow;
        var expires = now.AddMinutes(_options.AccessTokenExpirationMinutes);
        var expiresIn = (int)_options.AccessTokenExpirationMinutes * 60;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, principal.UserId.ToString()),
            new(ClaimTypes.NameIdentifier, principal.UserId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Email, principal.Email),
            new(ClaimTypes.Email, principal.Email),
        };

        foreach (var role in principal.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Convert.FromBase64String(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expires.UtcDateTime,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessToken(tokenString, expiresIn);
    }
}