namespace Identity.Application.Handlers;

using Identity.Application.Abstractions;
using Identity.Application.Authentication;
using Identity.Application.Commands;
using MediatR;
using Shared.Application.Abstractions;
using Shared.Domain.Common;

internal sealed class UpdatePermissionHandler : IRequestHandler<UpdatePermissionCommand, Result<PermissionResponse>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePermissionHandler(
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork)
    {
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PermissionResponse>> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
        if (permission is null)
            return Result<PermissionResponse>.Failure(new Error("Permission.NotFound", "Permission not found."));

        permission.Update(request.Description);
        await _permissionRepository.UpdateAsync(permission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PermissionResponse>.Success(new PermissionResponse(
            permission.Id,
            permission.Resource,
            permission.Action,
            permission.Description,
            permission.IsDeleted));
    }
}
