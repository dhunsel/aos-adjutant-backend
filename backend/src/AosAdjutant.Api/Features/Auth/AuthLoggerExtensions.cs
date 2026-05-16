namespace AosAdjutant.Api.Features.Auth;

public static partial class AuthLoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Error, Message = "OIDC remote failure")]
    public static partial void Log_OIDC_failure(this ILogger logger, Exception? ex);
}
