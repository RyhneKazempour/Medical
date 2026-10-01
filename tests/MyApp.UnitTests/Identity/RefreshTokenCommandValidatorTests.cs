namespace MyApp.UnitTests.Identity;

using FluentValidation.TestHelper;
using MyApp.Identity.Application.Commands;
using MyApp.Identity.Application.Validators;
using Xunit;

public class RefreshTokenCommandValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator;

    public RefreshTokenCommandValidatorTests()
    {
        _validator = new RefreshTokenCommandValidator();
    }

    [Fact]
    public void Validate_EmptyRefreshToken_ReturnsFailure()
    {
        var command = new RefreshTokenCommand("");
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
            .WithErrorMessage("Refresh token is required.");
    }

    [Fact]
    public void Validate_WhitespaceRefreshToken_ReturnsFailure()
    {
        var command = new RefreshTokenCommand("   ");
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
            .WithErrorMessage("Refresh token is required.");
    }

    [Fact]
    public void Validate_ValidRefreshToken_ReturnsSuccess()
    {
        var command = new RefreshTokenCommand("valid-refresh-token-string");
        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}