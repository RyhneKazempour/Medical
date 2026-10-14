namespace Identity.Application.Abstractions;

using Identity.Application.Authentication;
using Shared.Domain.Common;

public interface ITokenService
    {
        Task<TokenPair> GenerateTokenPairAsync(AccessTokenPrincipal principal, string? clientIpAddress = null);

        Task<Result<TokenPair>> RefreshTokenAsync(string refreshToken, string? clientIpAddress, CancellationToken cancellationToken);
    }
