namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Domain;

public sealed record RefreshTokenCommand(string RefreshToken, string? ClientIpAddress = null) : IRequest<Result<TokenPair>>;