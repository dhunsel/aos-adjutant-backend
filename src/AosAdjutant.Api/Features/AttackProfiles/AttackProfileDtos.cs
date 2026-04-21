using System.ComponentModel.DataAnnotations;
using AosAdjutant.Api.Features.WeaponEffects;

namespace AosAdjutant.Api.Features.AttackProfiles;

public sealed record AttackProfileResponseDto(
    int AttackProfileId,
    string Name,
    bool IsRanged,
    int? Range,
    string Attacks,
    int ToHit,
    int ToWound,
    int? Rend,
    string Damage,
    int UnitId,
    uint Version,
    List<WeaponEffectResponseDto> WeaponEffects
);

public sealed record CreateAttackProfileDto
{
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; init; }
    public required bool IsRanged { get; init; }
    public int? Range { get; init; }

    [StringLength(100, MinimumLength = 1)]
    public required string Attacks { get; init; }
    public required int ToHit { get; init; }
    public required int ToWound { get; init; }
    public int? Rend { get; init; }

    [StringLength(100, MinimumLength = 1)]
    public required string Damage { get; init; }
    public required IReadOnlyList<string> WeaponEffects { get; init; }
}

public sealed record ChangeAttackProfileDto
{
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; init; }
    public required bool IsRanged { get; init; }
    public int? Range { get; init; }

    [StringLength(100, MinimumLength = 1)]
    public required string Attacks { get; init; }
    public required int ToHit { get; init; }
    public required int ToWound { get; init; }
    public int? Rend { get; init; }

    [StringLength(100, MinimumLength = 1)]
    public required string Damage { get; init; }
    public required IReadOnlyList<string> WeaponEffects { get; init; }
    public required uint Version { get; init; }
}
