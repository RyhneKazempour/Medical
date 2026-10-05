using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Domain;

namespace MyApp.Identity.Application.Handlers;

internal sealed class ListUserRolesHandler : IRequestHandler<ListUserRolesQuery, Result<IReadOnlyList<UserRoleDto>>>
{
    private readonly IUserRoleRepository _userRoleRepository;

    public ListUserRolesHandler(IUserRoleRepository userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    public async Task<Result<IReadOnlyList<UserRoleDto>>> Handle(ListUserRolesQuery request, CancellationToken cancellationToken)
    {
        var userRoles = await _userRoleRepository.GetByUserIdIncludingRoleAsync(request.UserId, cancellationToken);

        var response = userRoles
            .Where(ur => ur.IsDeleted == false)
            .Select(ur => new UserRoleDto(
                ur.Id,
                ur.ScopeType,
                ur.ScopeId,
                ur.Role?.Name ?? ""))
            .ToList();

        return Result<IReadOnlyList<UserRoleDto>>.Success(response);
    }
}