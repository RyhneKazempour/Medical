using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Domain;

namespace MyApp.Identity.Application.Handlers;

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