namespace MyApp.Shared.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
    DateTimeOffset Now { get; }
}
