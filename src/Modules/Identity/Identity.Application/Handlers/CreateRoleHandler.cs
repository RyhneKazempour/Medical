using MediatR;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Domain.Entities;
using MyApp.Shared.Application.Abstractions;
using MyApp.Identity.Application.Authentication;
using MyApp.Shared.Domain;

namespace MyApp.Identity.Application.Handlers;

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