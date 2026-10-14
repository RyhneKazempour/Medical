using MediatR;
using Shared.Domain.Common;
using Shared.Application.Abstractions;
using Identity.Application.Abstractions;
using Identity.Application.Commands;

namespace Identity.Application.Handlers;

internal sealed class RestoreUserHandler : IRequestHandler<RestoreUserCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RestoreUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RestoreUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(new Error("User.NotFound", "User not found."));

        if (!user.IsDeleted)
            return Result.Success();

        user.IsDeleted = false;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
