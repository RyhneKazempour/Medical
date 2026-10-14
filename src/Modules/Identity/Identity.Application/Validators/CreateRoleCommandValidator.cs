using FluentValidation;
using Identity.Application.Abstractions;
using Identity.Application.Commands;

namespace Identity.Application.Validators;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    private readonly IRoleRepository _roleRepository;

    public CreateRoleCommandValidator(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Role name is required.")

            .MustAsync(async (name, cancellationToken) =>
                await _roleRepository.GetByNameAsync(name, cancellationToken) is null)
            .WithMessage("Role name already exists.");
    }
}