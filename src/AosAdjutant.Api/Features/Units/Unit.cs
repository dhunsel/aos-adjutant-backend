#pragma warning disable MA0048
using AosAdjutant.Api.Common;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Features.AttackProfiles;

namespace AosAdjutant.Api.Features.Units;

public sealed class Unit
{
    public int UnitId { get; set; }
    public required string Name { get; set; }
    public int Health { get; set; }
    public required string Move { get; set; }
    public int Save { get; set; }
    public int Control { get; set; }
    public int? WardSave { get; set; }
    public int FactionId { get; set; }
    public uint Version { get; set; }

    public ICollection<Ability> Abilities { get; } = new List<Ability>();
    public ICollection<AttackProfile> AttackProfiles { get; } = new List<AttackProfile>();
}

public static class UnitErrors
{
    public static readonly AppError NotFound = new(ErrorCode.NotFound, "Unit not found.");
    public static readonly AppError AlreadyExists = new(ErrorCode.UniqueKeyError, "Unit already exists.");

    public static readonly AppError Concurrency = new(
        ErrorCode.ConcurrencyError,
        "Unit was already modified in the background."
    );
}
