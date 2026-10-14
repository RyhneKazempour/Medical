namespace Identity.Infrastructure.Services;

using Identity.Application.Abstractions;
using Identity.Contracts.Abstractions;
using System.Threading;
using System.Threading.Tasks;

internal sealed class UserExistsChecker : IUserExistsChecker
{
    private readonly IUserRepository _userRepository;

    public UserExistsChecker(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> ExistsByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _userRepository.ExistsByIdAsync(userId, cancellationToken);
    }
}