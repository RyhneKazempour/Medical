namespace MyApp.Identity.Application.Authentication;

public sealed record TokenPair(
    AccessToken AccessToken,
    string RefreshToken,
    int RefreshTokenExpiresIn);