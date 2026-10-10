namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Domain;

public sealed record LoginCommand(string Email, string Password, string? ClientIpAddress = null) : IRequest<Result<TokenPair>>;