namespace MyApp.Identity.Application.Handlers;

using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Application.Abstractions;
using MyApp.Shared.Domain;

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