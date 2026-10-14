namespace Identity.Application.Commands;

using Identity.Application.Authentication;
using MediatR;
using Shared.Domain.Common;

public sealed record LoginCommand(string Email, string Password, string? ClientIpAddress = null) : IRequest<Result<TokenPair>>;
