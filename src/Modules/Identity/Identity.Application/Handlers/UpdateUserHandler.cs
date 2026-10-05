using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Domain;

namespace MyApp.Identity.Application.Handlers;

internal sealed class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Result<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<UserResponse>.Failure(new Error("User.NotFound", "User not found."));

        // Ensure non-null values for required fields
        var firstName = request.FirstName ?? user.FirstName;
        var lastName = request.LastName ?? user.LastName;

        var updateResult = user.UpdateProfile(
            firstName,
            lastName,
            request.Phone,
            request.Mobile);

        if (updateResult.IsFailure)
            return Result<UserResponse>.Failure(updateResult.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserResponse>.Success(new UserResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Phone,
            user.Mobile,
            user.IsActive,
            user.IsDeleted,
            user.UserRoles.Select(ur => new UserRoleResponse(
                ur.Role?.Name ?? "",
                ur.ScopeType,
                ur.ScopeId)).ToList()));
    }
}