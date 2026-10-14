using Identity.Application.Abstractions;
using Identity.Application.Commands;
using MediatR;
using Shared.Application.Abstractions;
using Shared.Domain.Common;

namespace Identity.Application.Handlers;

internal sealed class RestorePermissionHandler : IRequestHandler<RestorePermissionCommand, Result>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RestorePermissionHandler(
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork)
    {
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RestorePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
        if (permission is null)
            return Result.Failure(new Error("Permission.NotFound", "Permission not found."));

        if (!permission.IsDeleted)
            return Result.Failure(new Error("Permission.NotDeleted", "Permission is not deleted."));

        permission.Restore();
        await _permissionRepository.UpdateAsync(permission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
