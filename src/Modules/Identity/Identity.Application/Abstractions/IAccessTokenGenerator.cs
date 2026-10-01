namespace MyApp.Identity.Application.Abstractions;

using MyApp.Identity.Application.Authentication;

public interface IAccessTokenGenerator
{
    AccessToken GenerateToken(AccessTokenPrincipal principal);
}