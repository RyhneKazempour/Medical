namespace MyApp.Identity.Application.Abstractions;

using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Domain;

public interface ITokenService
{
    TokenPair GenerateTokenPair(AccessTokenPrincipal principal, string? clientIpAddress = null);

    Task<Result<TokenPair>> RefreshTokenAsync(string refreshToken, string? clientIpAddress, CancellationToken cancellationToken);
}