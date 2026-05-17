namespace AosAdjutant.Api.Features.Users;

public static partial class UserServiceLoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "User {UserId} created")]
    public static partial void Log_UserCreated(this ILogger logger, int userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "User {UserId} updated")]
    public static partial void Log_UserUpdated(this ILogger logger, int userId);
}
