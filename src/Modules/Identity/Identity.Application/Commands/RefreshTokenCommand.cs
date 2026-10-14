namespace Identity.Application.Commands;

using Identity.Application.Authentication;
using MediatR;

using Shared.Domain.Common;
public sealed record RefreshTokenCommand(string RefreshToken, string? ClientIpAddress = null) : IRequest<Result<TokenPair>>;
