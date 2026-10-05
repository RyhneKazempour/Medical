using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Domain;

namespace MyApp.Identity.Application.Handlers;

internal sealed class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, Result<RoleResponse>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleHandler(
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RoleResponse>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
            return Result<RoleResponse>.Failure(new Error("Role.NotFound", "Role not found."));

        var updateResult = role.Update(request.Name, request.IsActive);

        if (updateResult.IsFailure)
            return Result<RoleResponse>.Failure(updateResult.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<RoleResponse>.Success(new RoleResponse(
            role.Id,
            role.Name,
            role.Description,
            role.IsActive,
            role.IsDeleted,
            new List<string>()));
    }
}