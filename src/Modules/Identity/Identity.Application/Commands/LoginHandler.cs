namespace MyApp.Identity.Application.Commands;

using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Application.Abstractions;
using MyApp.Shared.Domain;

internal sealed class LoginHandler : IRequestHandler<LoginCommand, Result<TokenPair>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginHandler(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<TokenPair>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return Result<TokenPair>.Failure(new Error("Auth.InvalidCredentials", "Invalid email or password."));
        }

        if (!user.IsActive)
        {
            return Result<TokenPair>.Failure(new Error("Auth.InvalidCredentials", "Invalid email or password."));
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<TokenPair>.Failure(new Error("Auth.InvalidCredentials", "Invalid email or password."));
        }

        var userRoles = await _userRoleRepository.GetByUserIdIncludingRoleAsync(user.Id, cancellationToken);
        var roles = userRoles
            .Where(ur => ur.Role is not null)
            .Select(ur => ur.Role!.Name)
            .Distinct()
            .ToArray();

        var principal = new AccessTokenPrincipal(user.Id, user.Email, roles);
        var tokenPair = await _tokenService.GenerateTokenPairAsync(principal, request.ClientIpAddress);

        return Result<TokenPair>.Success(tokenPair);
    }
}