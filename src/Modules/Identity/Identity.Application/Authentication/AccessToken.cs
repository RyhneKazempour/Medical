namespace MyApp.Identity.Application.Authentication;

public sealed record AccessToken(
    string Token,
    int ExpiresIn);