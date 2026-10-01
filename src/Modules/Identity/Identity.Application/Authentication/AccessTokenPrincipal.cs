namespace MyApp.Identity.Application.Authentication;

public sealed record AccessTokenPrincipal(
    Guid UserId,
    string Email,
    IReadOnlyCollection<string> Roles);