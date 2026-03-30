namespace AosAdjutant.Api.Features.Factions;

public static partial class FactionServiceLoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Faction {FactionId} created")]
    public static partial void Log_FactionCreated(this ILogger logger, int factionId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Faction {FactionId} updated")]
    public static partial void Log_FactionUpdated(this ILogger logger, int factionId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Faction {FactionId} deleted")]
    public static partial void Log_FactionDeleted(this ILogger logger, int factionId);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Update for faction {FactionId} with version {Version} failed because of version mismatch"
    )]
    public static partial void Log_FactionConcurrencyError(this ILogger logger, int factionId, uint version);
}
