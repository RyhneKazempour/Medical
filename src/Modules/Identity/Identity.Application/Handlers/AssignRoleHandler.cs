using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Domain;

namespace MyApp.Identity.Application.Handlers;

internal sealed class AssignRoleHandler : IRequestHandler<AssignRoleCommand, Result<UserRoleDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRoleHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserRoleDto>> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        // Validate user exists and is active
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<UserRoleDto>.Failure(new Error("User.NotFound", "User not found."));

        if (!user.IsActive)
            return Result<UserRoleDto>.Failure(new Error("User.Inactive", "User is inactive."));

        // Validate role exists and is active
        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
            return Result<UserRoleDto>.Failure(new Error("Role.NotFound", "Role not found."));

        if (!role.IsActive)
            return Result<UserRoleDto>.Failure(new Error("Role.Inactive", "Role is inactive."));

        // Check for duplicate active assignment
        var existingAssignment = await _userRoleRepository.GetByUserRoleScopeAsync(
            request.UserId, request.RoleId, request.ScopeType, request.ScopeId, cancellationToken);

        if (existingAssignment is not null && existingAssignment.IsDeleted == false)
        {
            return Result<UserRoleDto>.Failure(new Error("UserRole.DuplicateAssignment", "User already has this role assignment."));
        }

        // Create or restore the assignment
        UserRole userRole;
        if (existingAssignment is not null && existingAssignment.IsDeleted == true)
        {
            // Restore the existing deleted assignment
            existingAssignment.Restore();
            userRole = existingAssignment;
        }
        else
        {
            // Create new assignment
            var createResult = UserRole.Create(request.UserId, request.RoleId, request.ScopeType, request.ScopeId);
            if (createResult.IsFailure)
                return Result<UserRoleDto>.Failure(createResult.Error);

            userRole = createResult.Value;
        }

        await _userRoleRepository.AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserRoleDto>.Success(new UserRoleDto(
            userRole.Id,
            userRole.ScopeType,
            userRole.ScopeId,
            userRole.Role?.Name ?? ""));
    }
}