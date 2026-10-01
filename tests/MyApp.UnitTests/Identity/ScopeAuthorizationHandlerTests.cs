namespace MyApp.UnitTests.Identity;

using System.Security.Claims;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Infrastructure.Authorization;
using MyApp.Identity.Domain.Entities;
using MyApp.Identity.Domain.ValueObjects;
using Xunit;

public class ScopeAuthorizationHandlerTests
{
    private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly ScopeAuthorizationHandler _handler;

    public ScopeAuthorizationHandlerTests()
    {
        _userRoleRepositoryMock = new Mock<IUserRoleRepository>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _handler = new ScopeAuthorizationHandler(
            _userRoleRepositoryMock.Object,
            _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task HandleRequirementAsync_UserWithMatchingScope_Succeeds()
    {
        var userId = Guid.NewGuid();
        var hospitalId = Guid.NewGuid();

        var roleResult = Role.Create("Doctor", null);
        var role = roleResult.Value;

        var userRoleResult = UserRole.CreateHospital(userId, role.Id, hospitalId);
        var userRole = userRoleResult.Value;
        userRole.GetType().GetProperty("Role")?.SetValue(userRole, role);

        _userRoleRepositoryMock.Setup(x => x.GetByUserIdIncludingRoleAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([userRole]);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["hospitalId"] = hospitalId.ToString();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var requirement = new ScopeRequirement(ScopeType.Hospital, "hospitalId", ["Doctor"], true);

        var context = new AuthorizationHandlerContext([requirement], new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId.ToString())])), null);

        await _handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_UserWithGlobalScopeAndAllowGlobal_Succeeds()
    {
        var userId = Guid.NewGuid();
        var hospitalId = Guid.NewGuid();

        var roleResult = Role.Create("SuperAdmin", null);
        var role = roleResult.Value;

        var userRoleResult = UserRole.CreateGlobal(userId, role.Id);
        var userRole = userRoleResult.Value;
        userRole.GetType().GetProperty("Role")?.SetValue(userRole, role);

        _userRoleRepositoryMock.Setup(x => x.GetByUserIdIncludingRoleAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([userRole]);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["hospitalId"] = hospitalId.ToString();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var requirement = new ScopeRequirement(ScopeType.Hospital, "hospitalId", ["SuperAdmin"], true);

        var context = new AuthorizationHandlerContext([requirement], new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId.ToString())])), null);

        await _handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_UserWithGlobalScopeButAllowGlobalFalse_Fails()
    {
        var userId = Guid.NewGuid();
        var hospitalId = Guid.NewGuid();

        var roleResult = Role.Create("SuperAdmin", null);
        var role = roleResult.Value;

        var userRoleResult = UserRole.CreateGlobal(userId, role.Id);
        var userRole = userRoleResult.Value;
        userRole.GetType().GetProperty("Role")?.SetValue(userRole, role);

        _userRoleRepositoryMock.Setup(x => x.GetByUserIdIncludingRoleAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([userRole]);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["hospitalId"] = hospitalId.ToString();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var requirement = new ScopeRequirement(ScopeType.Hospital, "hospitalId", ["SuperAdmin"], false);

        var context = new AuthorizationHandlerContext([requirement], new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId.ToString())])), null);

        await _handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_UserWithDifferentHospital_Fails()
    {
        var userId = Guid.NewGuid();
        var hospitalId = Guid.NewGuid();
        var otherHospitalId = Guid.NewGuid();

        var roleResult = Role.Create("Doctor", null);
        var role = roleResult.Value;

        var userRoleResult = UserRole.CreateHospital(userId, role.Id, otherHospitalId);
        var userRole = userRoleResult.Value;
        userRole.GetType().GetProperty("Role")?.SetValue(userRole, role);

        _userRoleRepositoryMock.Setup(x => x.GetByUserIdIncludingRoleAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([userRole]);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["hospitalId"] = hospitalId.ToString();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var requirement = new ScopeRequirement(ScopeType.Hospital, "hospitalId", ["Doctor"], true);

        var context = new AuthorizationHandlerContext([requirement], new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId.ToString())])), null);

        await _handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_UserWithWrongRole_Fails()
    {
        var userId = Guid.NewGuid();
        var hospitalId = Guid.NewGuid();

        var roleResult = Role.Create("Receptionist", null);
        var role = roleResult.Value;

        var userRoleResult = UserRole.CreateHospital(userId, role.Id, hospitalId);
        var userRole = userRoleResult.Value;
        userRole.GetType().GetProperty("Role")?.SetValue(userRole, role);

        _userRoleRepositoryMock.Setup(x => x.GetByUserIdIncludingRoleAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([userRole]);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["hospitalId"] = hospitalId.ToString();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var requirement = new ScopeRequirement(ScopeType.Hospital, "hospitalId", ["Doctor"], true);

        var context = new AuthorizationHandlerContext([requirement], new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId.ToString())])), null);

        await _handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_NoUserIdClaim_Fails()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["hospitalId"] = Guid.NewGuid().ToString();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var requirement = new ScopeRequirement(ScopeType.Hospital, "hospitalId", ["Doctor"], true);

        var context = new AuthorizationHandlerContext([requirement], new ClaimsPrincipal(
            new ClaimsIdentity()), null);

        await _handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_NoRouteValue_Fails()
    {
        var userId = Guid.NewGuid();

        var httpContext = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var requirement = new ScopeRequirement(ScopeType.Hospital, "hospitalId", ["Doctor"], true);

        var context = new AuthorizationHandlerContext([requirement], new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId.ToString())])), null);

        await _handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_EmptyUserRoles_Fails()
    {
        var userId = Guid.NewGuid();
        var hospitalId = Guid.NewGuid();

        _userRoleRepositoryMock.Setup(x => x.GetByUserIdIncludingRoleAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["hospitalId"] = hospitalId.ToString();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var requirement = new ScopeRequirement(ScopeType.Hospital, "hospitalId", ["Doctor"], true);

        var context = new AuthorizationHandlerContext([requirement], new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId.ToString())])), null);

        await _handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }
}