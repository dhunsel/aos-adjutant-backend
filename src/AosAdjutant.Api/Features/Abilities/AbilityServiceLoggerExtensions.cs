namespace AosAdjutant.Api.Features.Abilities;

public static partial class AbilityServiceLoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Generic ability {AbilityId} created")]
    public static partial void Log_GenericAbilityCreated(this ILogger logger, int abilityId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Ability {AbilityId} created for {ParentType} {ParentId}")]
    public static partial void Log_ScopedAbilityCreated(
        this ILogger logger,
        int abilityId,
        string parentType,
        int parentId
    );

    [LoggerMessage(Level = LogLevel.Information, Message = "Ability {AbilityId} updated")]
    public static partial void Log_AbilityUpdated(this ILogger logger, int abilityId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Ability {AbilityId} deleted")]
    public static partial void Log_AbilityDeleted(this ILogger logger, int abilityId);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Update for ability {AbilityId} with version {Version} failed because of version mismatch"
    )]
    public static partial void Log_AbilityConcurrencyError(this ILogger logger, int abilityId, uint version);
}
