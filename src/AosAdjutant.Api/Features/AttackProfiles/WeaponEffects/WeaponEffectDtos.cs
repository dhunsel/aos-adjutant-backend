namespace AosAdjutant.Api.Features.AttackProfiles.WeaponEffects;

public record WeaponEffectResponseDto(string Key, string Name);

public record CreateWeaponEffectDto(string Key, string Name);
