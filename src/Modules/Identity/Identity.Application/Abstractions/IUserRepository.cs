namespace Identity.Application.Abstractions;

using Identity.Domain.Entities;
using Shared.Application.Abstractions;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByMobileAsync(string mobile, CancellationToken cancellationToken = default);
    Task<bool> ExistsByMobileAsync(string mobile, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
