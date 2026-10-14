using MediatR;
using Shared.Domain.Common;
using Shared.Application.Abstractions;
using Identity.Application.Abstractions;
using Identity.Application.Commands;

namespace Identity.Application.Handlers;

internal sealed class RestoreRoleHandler : IRequestHandler<RestoreRoleCommand, Result>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RestoreRoleHandler(
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RestoreRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
            return Result.Failure(new Error("Role.NotFound", "Role not found."));

        if (!role.IsDeleted)
            return Result.Failure(new Error("Role.NotDeleted", "Role is not deleted."));

        role.IsDeleted = false;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
