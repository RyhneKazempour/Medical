namespace MyApp.Identity.Application.Authentication;

public sealed record PermissionResponse(
    Guid Id,
    string Resource,
    string Action,
    string? Description,
    bool IsDeleted);
