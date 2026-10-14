namespace Identity.Application.Abstractions;

using Identity.Application.Authentication;

public interface IAccessTokenGenerator
{
    AccessToken GenerateToken(AccessTokenPrincipal principal);
}