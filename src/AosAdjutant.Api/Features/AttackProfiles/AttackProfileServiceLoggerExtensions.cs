namespace AosAdjutant.Api.Features.AttackProfiles;

public static partial class AttackProfileServiceLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Attack profile {AttackProfileId} created for unit {UnitId}"
    )]
    public static partial void Log_AttackProfileCreated(this ILogger logger, int attackProfileId, int unitId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Attack profile {AttackProfileId} updated for unit {UnitId}"
    )]
    public static partial void Log_AttackProfileUpdated(this ILogger logger, int attackProfileId, int unitId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Attack profile {AttackProfileId} deleted")]
    public static partial void Log_AttackProfileDeleted(this ILogger logger, int attackProfileId);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message =
            "Update for attack profile {AttackProfileId} with version {Version} failed because of version mismatch"
    )]
    public static partial void Log_AttackProfileConcurrencyError(
        this ILogger logger,
        int attackProfileId,
        uint version
    );
}
