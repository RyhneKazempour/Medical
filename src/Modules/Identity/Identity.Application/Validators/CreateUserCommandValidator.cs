using FluentValidation;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Application.Commands;
using System.Text.RegularExpressions;

namespace MyApp.Identity.Application.Validators;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email is required and must be a valid email address.")

            .MustAsync(async (email, cancellationToken) =>
                await _userRepository.ExistsByEmailAsync(email, cancellationToken) is false)
            .WithMessage("Email already exists.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.");

        RuleFor(x => x.Mobile)
            .Must(mobile => string.IsNullOrEmpty(mobile) || Regex.IsMatch(mobile, @"^\+?[1-9]\d{1,14}$"))
            .WithMessage("Mobile number format is not valid.");
    }
}