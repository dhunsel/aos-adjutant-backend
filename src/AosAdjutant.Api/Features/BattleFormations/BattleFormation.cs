#pragma warning disable MA0048
using AosAdjutant.Api.Common;
using AosAdjutant.Api.Features.Abilities;

namespace AosAdjutant.Api.Features.BattleFormations;

public sealed class BattleFormation
{
    public int BattleFormationId { get; set; }
    public required string Name { get; set; }
    public int FactionId { get; set; }
    public uint Version { get; set; }

    public ICollection<Ability> Abilities { get; } = new List<Ability>();
}

public static class BattleFormationErrors
{
    public static readonly AppError NotFound = new(ErrorCode.NotFound, "Battle formation not found.");
    public static readonly AppError AlreadyExists = new(ErrorCode.UniqueKeyError, "Battle formation already exists.");

    public static readonly AppError Concurrency = new(
        ErrorCode.ConcurrencyError,
        "Battle formation was already modified in the background."
    );
}
