using AosAdjutant.Api.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.WeaponEffects;

[Route("api/weapon-effects")]
[ApiController]
[Tags("Weapon Effects")]
public sealed class WeaponEffectController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all weapon effects")]
    [ProducesResponseType<List<WeaponEffectResponseDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WeaponEffectResponseDto>>> GetWeaponEffects(
        [FromQuery] WeaponEffectQuery weaponEffectQuery
    )
    {
        var weaponEffects = await context
            .WeaponEffects.AsNoTracking()
            .ApplyFilters(weaponEffectQuery)
            .ApplySorting(weaponEffectQuery)
            .Select(we => new WeaponEffectResponseDto(we.Key, we.Name))
            .ToListAsync();
        return Ok(weaponEffects);
    }
}
