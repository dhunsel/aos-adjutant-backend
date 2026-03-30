namespace AosAdjutant.Api.Features.BattleFormations;

public static partial class BattleFormationServiceLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Battle formation {BattleFormationId} created for faction {FactionId}"
    )]
    public static partial void Log_BattleFormationCreated(this ILogger logger, int battleFormationId, int factionId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Battle formation {BattleFormationId} updated for faction {FactionId}"
    )]
    public static partial void Log_BattleFormationUpdated(this ILogger logger, int battleFormationId, int factionId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Battle formation {BattleFormationId} deleted")]
    public static partial void Log_BattleFormationDeleted(this ILogger logger, int battleFormationId);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message =
            "Update for battle formation {BattleFormationId} with version {Version} failed because of version mismatch"
    )]
    public static partial void Log_BattleFormationConcurrencyError(
        this ILogger logger,
        int battleFormationId,
        uint version
    );
}
