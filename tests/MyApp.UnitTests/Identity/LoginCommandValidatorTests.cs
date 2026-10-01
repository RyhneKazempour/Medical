namespace MyApp.UnitTests.Identity;

using FluentValidation.TestHelper;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Application.Validators;
using Xunit;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests()
    {
        _validator = new LoginCommandValidator();
    }

    [Fact]
    public void Validate_EmptyEmail_ReturnsFailure()
    {
        var command = new LoginCommand("", "password123");
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required.");
    }

    [Fact]
    public void Validate_InvalidEmailFormat_ReturnsFailure()
    {
        var command = new LoginCommand("not-an-email", "password123");
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public void Validate_EmptyPassword_ReturnsFailure()
    {
        var command = new LoginCommand("test@example.com", "");
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required.");
    }

    [Fact]
    public void Validate_PasswordTooShort_ReturnsFailure()
    {
        var command = new LoginCommand("test@example.com", "short");
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters.");
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var command = new LoginCommand("test@example.com", "password123");
        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}