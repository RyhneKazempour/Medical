using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Domain;

namespace MyApp.Identity.Application.Handlers;

internal sealed class SoftDeleteRoleHandler : IRequestHandler<SoftDeleteRoleCommand, Result>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeleteRoleHandler(
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SoftDeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
            return Result.Failure(new Error("Role.NotFound", "Role not found."));

        // Check if role can be deleted (no active user assignments)
        var userRoles = await _userRoleRepository.GetByRoleIdAsync(role.Id, cancellationToken);
        if (userRoles.Any(ur => ur.IsDeleted == false))
        {
            return Result.Failure(new Error("Role.HasActiveAssignments", "Cannot delete role with active assignments."));
        }

        role.IsDeleted = true;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}