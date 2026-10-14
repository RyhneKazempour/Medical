namespace Identity.Application.Handlers;

using Identity.Application.Abstractions;
using Identity.Application.Commands;
using MediatR;
using Shared.Application.Abstractions;
using Shared.Domain.Common;

internal sealed class SoftDeletePermissionHandler : IRequestHandler<SoftDeletePermissionCommand, Result>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeletePermissionHandler(
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork)
    {
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SoftDeletePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
        if (permission is null)
            return Result.Failure(new Error("Permission.NotFound", "Permission not found."));

        permission.SoftDelete();
        await _permissionRepository.UpdateAsync(permission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
