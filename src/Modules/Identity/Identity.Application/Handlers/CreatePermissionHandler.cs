namespace MyApp.Identity.Application.Handlers;

using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Domain;
using MyApp.Shared.Application.Abstractions;

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