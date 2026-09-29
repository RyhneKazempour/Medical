namespace MyApp.UnitTests.Medical;

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyApp.Identity.Application.Abstractions;
using MyApp.Medical.Application.Abstractions;
using MyApp.Medical.Application.Commands;
using MyApp.Medical.Application.Validators;
using MyApp.Medical.Domain.Entities;
using MyApp.Shared.Application.Abstractions;
using MyApp.Shared.Domain;
using Xunit;

public class CreateDoctorCommandValidatorTests
{
    private readonly CreateDoctorCommandValidator _validator;

    public CreateDoctorCommandValidatorTests()
    {
        _validator = new CreateDoctorCommandValidator();
    }

    [Fact]
    public void Validate_EmptyUserId_ReturnsFailure()
    {
        var command = new CreateDoctorCommand(Guid.Empty, "LIC12345", "Test bio");
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("User ID is required"));
    }

    [Fact]
    public void Validate_EmptyLicenseNumber_ReturnsFailure()
    {
        var command = new CreateDoctorCommand(Guid.NewGuid(), "", "Test bio");
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("License number is required"));
    }

    [Fact]
    public void Validate_LicenseNumberTooLong_ReturnsFailure()
    {
        var command = new CreateDoctorCommand(Guid.NewGuid(), new string('L', 51), "Test bio");
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_BioTooLong_ReturnsFailure()
    {
        var command = new CreateDoctorCommand(Guid.NewGuid(), "LIC12345", new string('B', 2001));
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var command = new CreateDoctorCommand(Guid.NewGuid(), "LIC12345", "Test bio");
        var result = _validator.Validate(command);
        Assert.True(result.IsValid);
    }
}

public class CreateDoctorHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateDoctorHandler _handler;

    public CreateDoctorHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateDoctorHandler(_doctorRepositoryMock.Object, _userRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_UserDoesNotExist_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var command = new CreateDoctorCommand(userId, "LIC12345", "Test bio");
        _userRepositoryMock.Setup(x => x.ExistsByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Doctor.UserNotFound", result.Error.Code);
        _doctorRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Doctor>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UserAlreadyHasDoctor_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var command = new CreateDoctorCommand(userId, "LIC12345", "Test bio");
        _userRepositoryMock.Setup(x => x.ExistsByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _doctorRepositoryMock.Setup(x => x.ExistsForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Doctor.AlreadyExists", result.Error.Code);
        _doctorRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Doctor>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidUserAndNoExistingDoctor_CreatesDoctor()
    {
        var userId = Guid.NewGuid();
        var command = new CreateDoctorCommand(userId, "LIC12345", "Test bio");
        _userRepositoryMock.Setup(x => x.ExistsByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _doctorRepositoryMock.Setup(x => x.ExistsForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        _doctorRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Doctor>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DatabaseUniqueConstraintViolation_ReturnsConflictError()
    {
        var userId = Guid.NewGuid();
        var command = new CreateDoctorCommand(userId, "LIC12345", "Test bio");
        _userRepositoryMock.Setup(x => x.ExistsByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _doctorRepositoryMock.Setup(x => x.ExistsForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("duplicate key value violates unique constraint \"uq_doctors_user_id\""));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Doctor.AlreadyExists", result.Error.Code);
    }
}