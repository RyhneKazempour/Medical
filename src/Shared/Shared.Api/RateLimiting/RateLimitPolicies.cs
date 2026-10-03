namespace MyApp.Shared.Api.RateLimiting;

public static class RateLimitPolicies
{
    public const string Default = "default";

    public const string IdentityAuth = "identity-auth";

    public const string IdentityRefresh = "identity-refresh";

    public const string IdentityOtp = "identity-otp";
}