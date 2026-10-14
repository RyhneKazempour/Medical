namespace Identity.Application.Handlers;

using Identity.Application.Abstractions;
using Identity.Application.Authentication;
using Identity.Application.Commands;
using Identity.Domain.Entities;
using MediatR;
using Shared.Application.Abstractions;
using Shared.Domain.Common;

internal sealed class CreatePermissionHandler : IRequestHandler<CreatePermissionCommand, Result<PermissionResponse>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePermissionHandler(
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork)
    {
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PermissionResponse>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
    {
        var permissionResult = Permission.Create(request.Resource, request.Action, request.Description);
        if (permissionResult.IsFailure)
            return Result<PermissionResponse>.Failure(permissionResult.Error);

        var existing = await _permissionRepository.GetByResourceActionAsync(
            permissionResult.Value.Resource, permissionResult.Value.Action, cancellationToken);
        if (existing is not null)
            return Result<PermissionResponse>.Failure(new Error("Permission.Duplicate", "A permission with this resource and action already exists."));

        await _permissionRepository.AddAsync(permissionResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PermissionResponse>.Success(new PermissionResponse(
            permissionResult.Value.Id,
            permissionResult.Value.Resource,
            permissionResult.Value.Action,
            permissionResult.Value.Description,
            permissionResult.Value.IsDeleted));
    }
}
