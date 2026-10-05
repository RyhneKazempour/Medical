namespace MyApp.Identity.Application.Authentication;

public sealed record UserRoleAssignmentResponse(
    Guid UserId,
    string UserEmail,
    IReadOnlyList<UserRoleResponse> Roles);