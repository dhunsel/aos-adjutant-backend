using AosAdjutant.Api.Database;
using AosAdjutant.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.AttackProfiles.WeaponEffects;

[Route("api/weapon-effects")]
[ApiController]
[Tags("Weapon Effects")]
public class WeaponEffectController(ApplicationDbContext context) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create a weapon effect")]
    [ProducesResponseType<WeaponEffectResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<WeaponEffectResponseDto>> CreateWeaponEffect(
        [FromBody] CreateWeaponEffectDto weaponEffectData
    )
    {
        var key = weaponEffectData.Key.ToLower();
        var isDuplicate = await context.WeaponEffects.AnyAsync(we => we.Name == weaponEffectData.Name || we.Key == key);
        if (isDuplicate)
            return this.ApiProblem(new AppError(ErrorCode.UniqueKeyError, "Weapon effect already exists."));

        var newWeaponEffect = new WeaponEffect { Key = key, Name = weaponEffectData.Name };

        context.WeaponEffects.Add(newWeaponEffect);
        await context.SaveChangesAsync();

        return Created(
            $"api/weapon-effects/{newWeaponEffect.Key}", // Expose id or remove key and only use id?
            new WeaponEffectResponseDto(newWeaponEffect.Key, newWeaponEffect.Name)
        );
    }

    [HttpGet]
    [EndpointSummary("Get all weapon effects")]
    [ProducesResponseType<List<WeaponEffectResponseDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WeaponEffectResponseDto>>> GetWeaponEffects()
    {
        var weaponEffects = await context.WeaponEffects
            .AsNoTracking()
            .Select(we => new WeaponEffectResponseDto(we.Key, we.Name))
            .ToListAsync();
        return Ok(weaponEffects);
    }

    [HttpDelete("{key}")]
    [EndpointSummary("Delete a weapon effect")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteWeaponEffect([FromRoute] string key)
    {
        var weaponEffect = await context.WeaponEffects.FirstOrDefaultAsync(we => we.Key == key);

        if (weaponEffect is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Weapon effect not found."));

        context.WeaponEffects.Remove(weaponEffect);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
