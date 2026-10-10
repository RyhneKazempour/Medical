namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Domain;

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