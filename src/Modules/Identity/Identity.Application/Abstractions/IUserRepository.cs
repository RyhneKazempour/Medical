namespace MyApp.Identity.Application.Abstractions;

using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Application.Abstractions;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}
