namespace MyApp.Identity.Application.Authentication;

public sealed record RoleResponse(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    bool IsDeleted,
    IReadOnlyList<string> UserCount); // Number of users with this role