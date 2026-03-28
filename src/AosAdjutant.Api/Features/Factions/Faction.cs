#pragma warning disable MA0048
using AosAdjutant.Api.Common;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Features.BattleFormations;
using AosAdjutant.Api.Features.Units;

namespace AosAdjutant.Api.Features.Factions;

public sealed class Faction
{
    public int FactionId { get; set; }
    public required string Name { get; set; }
    public uint Version { get; set; }

    public ICollection<BattleFormation> BattleFormations { get; } = new List<BattleFormation>();
    public ICollection<Unit> Units { get; } = new List<Unit>();
    public ICollection<Ability> Abilities { get; } = new List<Ability>();
}

public static class FactionErrors
{
    public static readonly AppError NotFound = new(ErrorCode.NotFound, "Faction not found.");
    public static readonly AppError AlreadyExists = new(ErrorCode.UniqueKeyError, "Faction already exists.");

    public static readonly AppError Concurrency = new(
        ErrorCode.ConcurrencyError,
        "Faction was already modified in the background."
    );
}
