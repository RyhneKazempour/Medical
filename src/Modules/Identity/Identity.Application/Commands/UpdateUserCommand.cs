namespace Identity.Application.Commands;

using Identity.Application.Authentication;
using MediatR;
using Shared.Domain.Common;

public sealed record UpdateUserCommand(
    Guid UserId,
    string? FirstName,
    string? LastName,
    string? Phone,
    string? Mobile) : IRequest<Result<UserResponse>>;
