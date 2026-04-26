using AosAdjutant.Api.Common;

namespace AosAdjutant.Api.Features.WeaponEffects;

public sealed record WeaponEffectResponseDto(string Key, string Name);

public sealed record WeaponEffectQuery : PagedQuery;
