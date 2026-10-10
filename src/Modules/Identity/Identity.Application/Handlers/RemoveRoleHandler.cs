using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Domain;

namespace MyApp.Identity.Application.Handlers;

internal sealed class RemoveRoleHandler : IRequestHandler<RemoveRoleCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveRoleHandler(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(new Error("User.NotFound", "User not found."));

        // Find the user role assignment
        var userRole = await _userRoleRepository.GetByUserRoleScopeAsync(
            request.UserId, request.RoleId, request.ScopeType, request.ScopeId, cancellationToken);

        if (userRole is null)
            return Result.Failure(new Error("UserRole.NotFound", "User role assignment not found."));

        // Soft delete the assignment
        userRole.SoftDelete();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}