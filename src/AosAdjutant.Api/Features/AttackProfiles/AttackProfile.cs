using AosAdjutant.Api.Features.AttackProfiles.WeaponEffects;

namespace AosAdjutant.Api.Features.AttackProfiles;

public class AttackProfile
{
    public int AttackProfileId { get; set; }
    public required string Name { get; set; }
    public required bool IsRanged { get; set; }
    public int? Range { get; set; }
    public required string Attacks { get; set; }
    public required int ToHit { get; set; }
    public required int ToWound { get; set; }
    public int? Rend { get; set; }
    public required string Damage { get; set; }
    public required int UnitId { get; set; }
    public uint Version { get; set; }

    public ICollection<WeaponEffect> WeaponEffects { init; get; } = new List<WeaponEffect>();
}
