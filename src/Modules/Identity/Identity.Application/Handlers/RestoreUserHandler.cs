using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Domain;

namespace MyApp.Identity.Application.Handlers;

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