using MediatR;
using Shared.Domain.Common;
using Identity.Application.Abstractions;
using Identity.Application.Commands;
using Identity.Application.Authentication;

namespace Identity.Application.Handlers;

internal sealed class ListUsersWithRoleHandler : IRequestHandler<ListUsersWithRoleQuery, Result<IReadOnlyList<UserResponse>>>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserRepository _userRepository;

    public ListUsersWithRoleHandler(
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<IReadOnlyList<UserResponse>>> Handle(ListUsersWithRoleQuery request, CancellationToken cancellationToken)
    {
        var userRoles = await _userRoleRepository.GetByRoleIdAsync(request.RoleId, cancellationToken);

        var userIds = userRoles
            .Where(ur => ur.IsDeleted == false)
            .Select(ur => ur.UserId)
            .ToList();

        var users = await _userRepository.GetAllAsync(cancellationToken);
        var userList = users
            .Where(u => userIds.Contains(u.Id) && u.IsActive && u.IsDeleted == false)
            .ToList();

        var response = userList.Select(u => new UserResponse(
            u.Id,
            u.Email,
            u.FirstName,
            u.LastName,
            u.Phone,
            u.Mobile,
            u.IsActive,
            u.IsDeleted,
            new List<UserRoleResponse>())).ToList();

        return Result<IReadOnlyList<UserResponse>>.Success(response);
    }
}
