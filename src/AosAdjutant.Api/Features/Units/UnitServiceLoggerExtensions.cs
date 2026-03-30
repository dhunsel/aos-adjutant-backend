namespace AosAdjutant.Api.Features.Units;

public static partial class UnitServiceLoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Unit {UnitId} created for faction {FactionId}")]
    public static partial void Log_UnitCreated(this ILogger logger, int unitId, int factionId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Unit {UnitId} updated for faction {FactionId}")]
    public static partial void Log_UnitUpdated(this ILogger logger, int unitId, int factionId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Unit {UnitId} deleted")]
    public static partial void Log_UnitDeleted(this ILogger logger, int unitId);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Update for unit {UnitId} with version {Version} failed because of version mismatch"
    )]
    public static partial void Log_UnitConcurrencyError(this ILogger logger, int unitId, uint version);
}
