namespace AosAdjutant.Api.Features.WeaponEffects;

public sealed class WeaponEffect
{
    public int WeaponEffectId { get; set; }
    public required string Key { get; set; }
    public required string Name { get; set; }
}
