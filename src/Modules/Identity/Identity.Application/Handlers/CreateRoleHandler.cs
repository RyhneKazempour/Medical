using MediatR;
using Shared.Domain.Common;
using Shared.Application.Abstractions;
using Identity.Application.Abstractions;
using Identity.Application.Authentication;
using Identity.Application.Commands;
using Identity.Domain.Entities;

namespace Identity.Application.Handlers;

internal sealed class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Result<RoleResponse>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleHandler(
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RoleResponse>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var result = Role.Create(request.Name, request.Description);

        if (result.IsFailure)
            return Result<RoleResponse>.Failure(result.Error);

        var role = result.Value;

        await _roleRepository.AddAsync(role, cancellationToken);
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