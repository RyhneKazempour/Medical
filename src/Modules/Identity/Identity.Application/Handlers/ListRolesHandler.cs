using MediatR;
using Shared.Domain.Common;
using Identity.Application.Abstractions;
using Identity.Application.Commands;
using Identity.Application.Authentication;

namespace Identity.Application.Handlers;

internal sealed class ListRolesHandler : IRequestHandler<ListRolesQuery, Result<IReadOnlyList<RoleResponse>>>
{
    private readonly IRoleRepository _roleRepository;

    public ListRolesHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result<IReadOnlyList<RoleResponse>>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);

        var response = roles.Select(r => new RoleResponse(
            r.Id,
            r.Name,
            r.Description,
            r.IsActive,
            r.IsDeleted,
            new List<string>())).ToList();

        return Result<IReadOnlyList<RoleResponse>>.Success(response);
    }
}
