using AosAdjutant.Api.Common;
using AosAdjutant.Api.Database;
using Microsoft.AspNetCore.Authorization;
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
    [ProducesResponseType<PaginatedResponse<WeaponEffectResponseDto>>(StatusCodes.Status200OK)]
    [Authorize]
    public async Task<ActionResult<PaginatedResponse<WeaponEffectResponseDto>>> GetWeaponEffects(
        [FromQuery] WeaponEffectQuery weaponEffectQuery
    )
    {
        var weaponEffects = await context
            .WeaponEffects.AsNoTracking()
            .ApplyFilters(weaponEffectQuery)
            .ApplySorting(weaponEffectQuery)
            .Select(we => new WeaponEffectResponseDto(we.Key, we.Name))
            .ToPaginatedReponse(weaponEffectQuery);
        return Ok(weaponEffects);
    }
}
