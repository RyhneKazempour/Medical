namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;
using MyApp.Identity.Application.Authentication;

public sealed record CreateUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? Phone,
    string? Mobile) : IRequest<Result<UserResponse>>;