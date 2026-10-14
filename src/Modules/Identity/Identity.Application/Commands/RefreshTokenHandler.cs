namespace Identity.Application.Commands;

using Identity.Application.Abstractions;
using Identity.Application.Authentication;
using MediatR;
using Shared.Domain.Common;

internal sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<TokenPair>>
{
    private readonly ITokenService _tokenService;

    public RefreshTokenHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<Result<TokenPair>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _tokenService.RefreshTokenAsync(request.RefreshToken, request.ClientIpAddress, cancellationToken);
    }
}
