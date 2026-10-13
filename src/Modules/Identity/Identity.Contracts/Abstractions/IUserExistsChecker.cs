namespace MyApp.Identity.Contracts.Abstractions;

using MyApp.Shared.Application.Abstractions;
using System.Threading;
using System.Threading.Tasks;

public interface IUserExistsChecker
{
    Task<bool> ExistsByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}