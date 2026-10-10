using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Domain;

namespace MyApp.Identity.Application.Handlers;

internal sealed class SoftDeleteUserHandler : IRequestHandler<SoftDeleteUserCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeleteUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SoftDeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(new Error("User.NotFound", "User not found."));

        user.SoftDelete();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}