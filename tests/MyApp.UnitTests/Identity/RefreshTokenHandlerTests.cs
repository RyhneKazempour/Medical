namespace MyApp.UnitTests.Identity;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Infrastructure.Authentication;
using MyApp.Shared.Application.Abstractions;
using MyApp.Shared.Domain;
using Xunit;

public class RefreshTokenHandlerTests
{
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly RefreshTokenHandler _handler;

    public RefreshTokenHandlerTests()
    {
        _tokenServiceMock = new Mock<ITokenService>();
        _handler = new RefreshTokenHandler(_tokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRefreshToken_ReturnsNewTokenPair()
    {
        var oldRefreshToken = "old-refresh-token";
        var newRefreshToken = "new-refresh-token";
        var accessToken = new AccessToken("new-access-token", 1800);
        var tokenPair = new TokenPair(accessToken, newRefreshToken, 7 * 24 * 60 * 60);

        var command = new RefreshTokenCommand(oldRefreshToken);
        _tokenServiceMock.Setup(x => x.RefreshTokenAsync(oldRefreshToken, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TokenPair>.Success(tokenPair));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("new-access-token", result.Value.AccessToken.Token);
        Assert.Equal(newRefreshToken, result.Value.RefreshToken);
        _tokenServiceMock.Verify(x => x.RefreshTokenAsync(oldRefreshToken, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRefreshToken_ReturnsFailure()
    {
        var oldRefreshToken = "invalid-refresh-token";
        var command = new RefreshTokenCommand(oldRefreshToken);
        _tokenServiceMock.Setup(x => x.RefreshTokenAsync(oldRefreshToken, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TokenPair>.Failure(new Error("Token.InvalidRefreshToken", "Invalid refresh token.")));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Token.InvalidRefreshToken", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ExpiredRefreshToken_ReturnsFailure()
    {
        var oldRefreshToken = "expired-refresh-token";
        var command = new RefreshTokenCommand(oldRefreshToken);
        _tokenServiceMock.Setup(x => x.RefreshTokenAsync(oldRefreshToken, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TokenPair>.Failure(new Error("Token.RefreshTokenExpired", "Refresh token has expired or been revoked.")));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Token.RefreshTokenExpired", result.Error.Code);
    }
}